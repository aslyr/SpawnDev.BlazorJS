(() => {
    if (globalThis.JSInterop) return;
    const JSInterop = {};
    globalThis.JSInterop = JSInterop;
    JSInterop.globalThis = globalThis;
    JSInterop.debugLevel = 0;
    JSInterop.pathObjectInfo = function (rootObject, path) {
        if (rootObject === null || rootObject === void 0) {
            // callers must call with the globalThis if they wish to use it as the rootObject.
            throw new DOMException('JSInterop.pathObjectInfo error: rootObject cannot be null');
        }
        var parent = rootObject;
        var target;
        var propertyName;
        if (path === null || path === void (0)) {
            // undefined and null can actually be property keys but that fact is ignored here atm
            target = parent;
            parent = null;
        } else if (typeof path === 'string' && typeof parent[path] === 'undefined') {
            var parts = path.split('.');
            propertyName = parts[parts.length - 1];
            for (var i = 0; i < parts.length - 1; i++) {
                parent = parent[parts[i]];
            }
            target = parent[propertyName];
        }
        else {
            propertyName = path;
            target = parent[propertyName];
        }
        var targetType = typeof target;
        var exists = targetType !== 'undefined' || (parent && propertyName in parent);
        return {
            parent,         // may be null even if target exists (Ex. if no path was given)
            propertyName,   // may be null even if target exists (Ex. if no path was given)
            target,         // may be undefined if it does not currently exist
            targetType,     // will always be a string of the target type (Ex. 'undefined', 'object', 'string', 'number')
            exists,         // will always be a bool value (true or false)
        };
    };

    JSInterop.pathToTarget = function (rootObject, path) {
        return JSInterop.pathObjectInfo(rootObject, path).target;
    };

    JSInterop.pathToParent = function (rootObject, path) {
        return JSInterop.pathObjectInfo(rootObject, path).parent;
    };

    JSInterop.setPromiseThenCatch = function (promise, thenCallback, catchCallback) {
        promise.then(thenCallback).catch(function (ex) {
            var err = "";
            if (ex) {
                if (typeof ex == 'string') {
                    err = ex;
                } else if (typeof ex == 'object') {
                    if (ex.constructor.name == 'Error') {
                        err = ex.message;
                    }
                }
            }
            if (!err) err = 'unknown error';
            catchCallback(err);
        });
    };

    JSInterop.__equals = function (obj1, obj2) {
        return obj1 === obj2;
    };

    JSInterop._returnMe = function (obj, returnType) {
        return serializeToDotNet(obj, returnType);
    };

    JSInterop._returnNew = function (obj, className, args, returnType) {
        if (obj === null) obj = globalThis;
        var { target, parent, targetType } = JSInterop.pathObjectInfo(obj, className);
        var ret = !args ? new target() : new target(...args);
        return serializeToDotNet(ret, returnType);
    };

    /**
     * returns all properties of the target object (except symbols.) number properties are returned as strings.
     * uses a combination of Reflect.ownKeys and the in operator as neither returns all properties of an object
     * @param {any} obj
     * @param {any} name
     * @param {any} hasOwnProperty
     * @returns string[]
     */
    JSInterop._getPropertyNames = function (obj, name, hasOwnProperty) {
        var target = JSInterop.pathToTarget(obj, name);
        var keys = [];
        if (hasOwnProperty && target.hasOwnProperty) {
            for (var k in target) {
                if (target.hasOwnProperty(k)) {
                    keys.push(k);
                }
            }
            var ownKeys = Reflect.ownKeys(target).filter(o => typeof o === 'string');
            for (var k of ownKeys) {
                if (keys.indexOf(k) == -1 && target.hasOwnProperty(k)) {
                    keys.push(k);
                }
            }
        } else {
            for (var k in target) {
                keys.push(k);
            }
            var ownKeys = Reflect.ownKeys(target).filter(o => typeof o === 'string');
            for (var k of ownKeys) {
                if (keys.indexOf(k) == -1) {
                    keys.push(k);
                }
            }
        }
        return keys;
    };

    JSInterop._instanceof = function (obj, name) {
        var target = JSInterop.pathToTarget(obj, name);
        return target && target.constructor ? target.constructor.name : '';
    };

    JSInterop._typeof = function (obj, name) {
        var target = JSInterop.pathToTarget(obj, name);
        return typeof target;
    };

    JSInterop._delete = function (obj, name) {
        var info = JSInterop.pathObjectInfo(obj, name);
        const parent = info.parent;
        const propertyName = info.propertyName;
        var ret = delete parent[propertyName];
        return ret;
    };

    function wrapJSObject(o) {
        return { __wrappedJSObject: o };
    }

    function createJSObjectReferenceMustWrapType(typeofValue) {
        switch (typeofValue) {
            case 'object':
                return false;
                break;
            case 'symbol':
            case 'function':
            case 'string':
                return true;
                break;
            default:
                return true;
                break;
        }
    }

    function serializeToDotNet(value, returnType) {
        var typeOfValue = typeof value;
        if (typeOfValue === 'undefined') {
            typeOfValue = 'object';
            value = null;
        }
        if (!returnType) {
            return value;
        }
        var isOverridden = returnType >= 128;
        var desiredType = isOverridden ? returnType - 128 : returnType;
        switch (desiredType) {
            case DotNet.JSCallResultType.Default:
                break;
            case DotNet.JSCallResultType.JSObjectReference:
                if (createJSObjectReferenceMustWrapType(typeOfValue)) {
                    value = wrapJSObject(value);
                }
                break;
            case DotNet.JSCallResultType.JSStreamReference:
                break;
            case DotNet.JSCallResultType.JSVoidResult:
                break;
            default:
                break;
        }
        if (!isOverridden) {
            return value;
        }
        // overridden so serialize it here
        switch (desiredType) {
            case DotNet.JSCallResultType.Default:
                return value;
            case DotNet.JSCallResultType.JSObjectReference:
                return value === null ? null : DotNet.createJSObjectReference(value);
            case DotNet.JSCallResultType.JSStreamReference:
                {
                    // TODO test and fix if needed
                    const n = DotNet.createJSStreamReference(value);
                    const r = JSON.stringify(n);
                    return BINDING.js_string_to_mono_string(r)
                }
            case DotNet.JSCallResultType.JSVoidResult:
                return null;
            default:
                throw new Error(`Invalid JS call result type '${a}'.`)
        }
    }

    JSInterop._returnArrayJSObjectReferenceIds = function (obj) {
        if (obj === null || obj === void 0) return null;
        var ret = [];
        for (var o of obj) ret.push(o === null || o === void 0 ? -1 : DotNet.createJSObjectReference(o).__jsObjectId);
        return ret;
    };

    // Instance
    JSInterop._set = function (obj, identifier, value) {
        if (obj === void 0 || obj === null) throw 'obj null or undefined';
        var pathInfo = JSInterop.pathObjectInfo(obj, identifier);
        if (!pathInfo.exists) {
            if (JSInterop.debugLevel > 0) {
                var targetType = pathInfo.parent ? pathInfo.parent.constructor.name : '[NULL]';
                console.log('NOTICE: JSInterop._set - property being set does not exist', targetType, identifier);
            }
        }
        pathInfo.parent[pathInfo.propertyName] = value;
    };

    JSInterop._get = function (obj, identifier, returnType) {
        var ret = null;
        if (obj === void 0 || obj === null) throw 'obj null or undefined';
        var { target, parent, targetType } = JSInterop.pathObjectInfo(obj, identifier);
        if (targetType === "function") {
            ret = target.bind(parent);
        } else {
            ret = target;
        }
        return serializeToDotNet(ret, returnType);
    };

    JSInterop._call = function (obj, identifier, args, returnType) {
        var ret = null;
        if (obj === void 0 || obj === null) throw 'obj null or undefined';
        var { target, parent, targetType } = JSInterop.pathObjectInfo(obj, identifier);
        if (targetType === "function") {
            //var fnBound = target.bind(parent);
            //ret = fnBound.apply(null, args);
            ret = target.apply(parent, args);
        } else {
            throw 'Call target is not a function';
        }
        return serializeToDotNet(ret, returnType);
    };

    //Global
    JSInterop._instanceofGlobal = function (identifier) {
        return JSInterop._instanceof(globalThis, identifier);
    };

    JSInterop._typeofGlobal = function (identifier) {
        return JSInterop._typeof(globalThis, identifier);
    };

    JSInterop._setGlobal = function (identifier, value) {
        JSInterop._set(globalThis, identifier, value);
    };

    JSInterop._deleteGlobal = function (identifier) {
        JSInterop._delete(globalThis, identifier);
    };

    JSInterop._getGlobal = function (identifier, returnType) {
        return JSInterop._get(globalThis, identifier, returnType);
    };

    JSInterop._callGlobal = function (identifier, args, returnType) {
        return JSInterop._call(globalThis, identifier, args, returnType);
    };

    const callbacks = {};
    JSInterop.DisposeCallbacker = function (callbackerID) {
        if (callbacks[callbackerID]) delete callbacks[callbackerID];
    };
    DotNet.attachReviver(function (key, value) {
        if (value && typeof value === 'object') {
            if ('_callbackId' in value) {
                var callbackId = value._callbackId;
                var callback = callbacks[callbackId];
                if (callback) return callback;
                callback = function fn() {
                    if (callback !== callbacks[callbackId]) {
                        console.warn('Disposed callback called.');
                        return;
                    }
                    var ret = null;
                    var args = ["Invoke"];
                    var paramTypes = value._paramTypes;
                    for (var i = 0; i < paramTypes.length; i++) {
                        var v = i < arguments.length ? arguments[i] : null;
                        var jsCallResultType = paramTypes[i];
                        v = serializeToDotNet(v, jsCallResultType);
                        args.push(v);
                    }
                    try {
                        ret = value._callback.invokeMethod.apply(value._callback, args);
                        if (!value._returnVoid) return ret;
                    } catch (ex) {
                        console.error('Callback invokeMethod error:', args, ret, ex);
                        //value.isDisposed = true;
                        //DisposeCallbacker(callbackId);
                    }
                };
                callbacks[callbackId] = callback;
                return callback; 
            }
            else if ('__wrappedJSObject' in value) {
                return value.__wrappedJSObject;
            }
            else if ('__undefinedref__' in value) {
                return;
            }
        }
        return value;
    });
})();