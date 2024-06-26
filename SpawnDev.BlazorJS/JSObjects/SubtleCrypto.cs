﻿using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.JSObjects
{
    /// <summary>
    /// The Crypto.subtle read-only property returns a SubtleCrypto which can then be used to perform low-level cryptographic operations.<br />
    /// https://developer.mozilla.org/en-US/docs/Web/API/SubtleCrypto<br />
    /// </summary>
    public class SubtleCrypto : JSObject
    {
        #region Constructors
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public SubtleCrypto(IJSInProcessObjectReference _ref) : base(_ref) { }
        #endregion

        #region Methods
        /// <summary>
        /// The decrypt() method of the SubtleCrypto interface decrypts some encrypted data. It takes as arguments a key to decrypt with, some optional extra parameters, and the data to decrypt (also known as "ciphertext"). It returns a Promise which will be fulfilled with the decrypted data (also known as "plaintext").
        /// </summary>
        /// <param name="algorithm">An object specifying the algorithm to be used, and any extra parameters as required. The values given for the extra parameters must match those passed into the corresponding encrypt() call.</param>
        /// <param name="key">A CryptoKey containing the key to be used for decryption. If using RSA-OAEP, this is the privateKey property of the CryptoKeyPair object.</param>
        /// <param name="data">An ArrayBuffer, a TypedArray, or a DataView containing the data to be decrypted (also known as ciphertext).</param>
        /// <returns></returns>
        public Task<ArrayBuffer> Decrypt(EncryptParams algorithm, CryptoKey key, Union<ArrayBuffer, TypedArray, DataView, byte[]> data) => JSRef.CallAsync<ArrayBuffer>("decrypt", algorithm, key, data);
        //public void DeriveBits() => JSRef.CallVoid("deriveBits");
        //public void DeriveKey() => JSRef.CallVoid("deriveKey");
        //public void Digest() => JSRef.CallVoid("digest");
        /// <summary>
        /// The encrypt() method of the SubtleCrypto interface encrypts data. It takes as its arguments a key to encrypt with, some algorithm-specific parameters, and the data to encrypt(also known as "plaintext"). It returns a Promise which will be fulfilled with the encrypted data(also known as "ciphertext").
        /// </summary>
        public Task<ArrayBuffer> Encrypt(EncryptParams algorithm, CryptoKey key, Union<ArrayBuffer, TypedArray, DataView, byte[]> data) => JSRef.CallAsync<ArrayBuffer>("encrypt", algorithm, key, data);
        /// <summary>
        /// The exportKey() method of the SubtleCrypto interface exports a key: that is, it takes as input a CryptoKey object and gives you the key in an external, portable format.
        /// To export a key, the key must have CryptoKey.extractable set to true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format">raw, pkcs8, spki, or jwk</param>
        /// <param name="key">the CryptoKey to export</param>
        /// <returns></returns>
        public Task<T> ExportKey<T>(string format, CryptoKey key) => JSRef.CallAsync<T>("exportKey", format, key);
        /// <summary>
        /// Export key in SubjectPublicKeyInfo format.<br />
        /// You can use this format to import or export RSA or Elliptic Curve public keys.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<ArrayBuffer> ExportKeySpki(CryptoKey key) => JSRef.CallAsync<ArrayBuffer>("exportKey", "spki", key);
        /// <summary>
        /// Export key in PKCS #8 format.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<ArrayBuffer> ExportKeyPkcs8(CryptoKey key) => JSRef.CallAsync<ArrayBuffer>("exportKey", "pkcs8", key);
        /// <summary>
        /// You can use this format to import or export AES or HMAC secret keys, or Elliptic Curve public keys.<br />
        /// In this format the key is supplied as an ArrayBuffer containing the raw bytes for the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<ArrayBuffer> ExportKeyRaw(CryptoKey key) => JSRef.CallAsync<ArrayBuffer>("exportKey", "raw", key);
        /// <summary>
        /// You can use JSON Web Key format to import or export RSA or Elliptic Curve public or private keys, as well as AES and HMAC secret keys.<br />
        /// JSON Web Key format is defined in RFC 7517. It describes a way to represent public, private, and secret keys as JSON objects.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<JSObject> ExportKeyJwk(CryptoKey key) => JSRef.CallAsync<JSObject>("exportKey", "jwk", key);
        /// <summary>
        /// Use the generateKey() method of the SubtleCrypto interface to generate a new key (for symmetric algorithms) or key pair (for public-key algorithms).
        /// </summary>
        /// <param name="algorithm">
        /// An object defining the type of key to generate and providing extra algorithm-specific parameters.<br />
        /// For RSASSA-PKCS1-v1_5, RSA-PSS, or RSA-OAEP: pass an RsaHashedKeyGenParams object.<br />
        /// For ECDSA or ECDH: pass an EcKeyGenParams object.<br />
        /// For HMAC: pass an HmacKeyGenParams object.<br />
        /// For AES-CTR, AES-CBC, AES-GCM, or AES-KW: pass an AesKeyGenParams object.
        /// </param>
        /// <param name="extractable">A boolean value indicating whether it will be possible to export the key using SubtleCrypto.exportKey() or SubtleCrypto.wrapKey().</param>
        /// <param name="keyUsages">An Array indicating what can be done with the key. Possible array values are: encrypt, decrypt, sign, verify, deriveKey, deriveBits, wrapKey, or unwrapKey.</param>
        /// <returns></returns>
        public Task<T> GenerateKey<T>(Union<RsaHashedKeyGenParams, EcKeyGenParams, HmacKeyGenParams, AesKeyGenParams> algorithm, bool extractable, IEnumerable<string> keyUsages) where T : CryptoKeyBase => JSRef.CallAsync<T>("generateKey", algorithm, extractable, keyUsages);
        /// <summary>
        /// The importKey() method of the SubtleCrypto interface imports a key: that is, it takes as input a key in an external, portable format and gives you a CryptoKey object that you can use in the Web Crypto API.
        /// </summary>
        /// <param name="format">A string describing the data format of the key to import. It can be one of the following: raw, pkcs8, spki, jwk</param>
        /// <param name="keyData">An ArrayBuffer, a TypedArray, a DataView, or a JSONWebKey object containing the key in the given format.</param>
        /// <param name="algorithm">An object defining the type of key to import and providing extra algorithm-specific parameters. CryptoImportParams, RsaHashedImportParams, EcKeyImportParams, HmacImportParams, or a string.</param>
        /// <param name="extractable">A boolean value indicating whether it will be possible to export the key using SubtleCrypto.exportKey() or SubtleCrypto.wrapKey().</param>
        /// <param name="keyUsages">An Array indicating what can be done with the key. Possible array values are: encrypt, decrypt, sign, verify, deriveKey, deriveBits, wrapKey, or unwrapKey.</param>
        /// <returns></returns>
        public Task<CryptoKey> ImportKey(string format, Union<ArrayBuffer, TypedArray, DataView, byte[]> keyData, Union<CryptoImportParams, string> algorithm, bool extractable, IEnumerable<string> keyUsages) => JSRef.CallAsync<CryptoKey>("importKey", format, keyData, algorithm, extractable, keyUsages);
        /// <summary>
        /// The sign() method of the SubtleCrypto interface generates a digital signature.<br />
        /// It takes as its arguments a key to sign with, some algorithm-specific parameters, and the data to sign. It returns a Promise which will be fulfilled with the signature.
        /// </summary>
        /// <param name="algorithm">A string or object that specifies the signature algorithm to use and its parameters</param>
        /// <param name="key">A CryptoKey object containing the key to be used for signing. If algorithm identifies a public-key cryptosystem, this is the private key.</param>
        /// <param name="data">An ArrayBuffer, a TypedArray or a DataView object containing the data to be signed.</param>
        /// <returns>A Promise that fulfills with an ArrayBuffer containing the signature.</returns>
        public Task<ArrayBuffer> Sign(Union<CryptoSignParams, string> algorithm, CryptoKey key, Union<ArrayBuffer, TypedArray, DataView, byte[]> data) => JSRef.CallAsync<ArrayBuffer>("sign", algorithm, key, data);
        /// <summary>
        /// The verify() method of the SubtleCrypto interface verifies a digital signature.<br />
        /// It takes as its arguments a key to verify the signature with, some algorithm-specific parameters, the signature, and the original signed data.It returns a Promise which will be fulfilled with a boolean value indicating whether the signature is valid.
        /// </summary>
        /// <param name="algorithm">A string or object defining the algorithm to use, and for some algorithm choices, some extra parameters. The values given for the extra parameters must match those passed into the corresponding sign() call.</param>
        /// <param name="key">A CryptoKey containing the key that will be used to verify the signature. It is the secret key for a symmetric algorithm and the public key for a public-key system.</param>
        /// <param name="signature">A ArrayBuffer containing the signature to verify.</param>
        /// <param name="data">A ArrayBuffer containing the data whose signature is to be verified.</param>
        /// <returns>A Promise that fulfills with a boolean value: true if the signature is valid, false otherwise.</returns>
        public Task<bool> Verify(Union<CryptoSignParams, string> algorithm, CryptoKey key, ArrayBuffer signature, ArrayBuffer data) => JSRef.CallAsync<bool>("verify", algorithm, key, signature, data);
        /// <summary>
        /// The unwrapKey() method of the SubtleCrypto interface "unwraps" a key. This means that it takes as its input a key that has been exported and then encrypted (also called "wrapped"). It decrypts the key and then imports it, returning a CryptoKey object that can be used in the Web Crypto API.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="wrappedKey"></param>
        /// <param name="unwrappingKey"></param>
        /// <param name="unwrapAlgo"></param>
        /// <param name="unwrappedKeyAlgo"></param>
        /// <param name="extractable"></param>
        /// <param name="keyUsages"></param>
        public Task<CryptoKey> UnwrapKey(string format, ArrayBuffer wrappedKey, CryptoKey unwrappingKey, EncryptParams unwrapAlgo, CryptoImportParams unwrappedKeyAlgo, bool extractable, IEnumerable<string> keyUsages) => JSRef.CallAsync<CryptoKey>("unwrapKey", format, wrappedKey, unwrappingKey, unwrapAlgo, unwrappedKeyAlgo, extractable, keyUsages);
        /// <summary>
        /// The wrapKey() method of the SubtleCrypto interface "wraps" a key. This means that it exports the key in an external, portable format, then encrypts the exported key. Wrapping a key helps protect it in untrusted environments, such as inside an otherwise unprotected data store or in transmission over an unprotected network.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="key"></param>
        /// <param name="wrappingKey"></param>
        /// <param name="wrapAlgo"></param>
        /// <returns></returns>
        public Task<ArrayBuffer> WrapKey(string format, CryptoKey key, CryptoKey wrappingKey, EncryptParams wrapAlgo) => JSRef.CallAsync<ArrayBuffer>("wrapKey", format, key, wrappingKey, wrapAlgo);
        #endregion
    }
}
