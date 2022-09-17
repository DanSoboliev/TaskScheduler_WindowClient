using System;
using System.Security.Cryptography;
using System.Text;
using TaskShedulerDesktopClient.Models;

namespace TaskShedulerDesktopClient.Data {
    public static class СryptographyData {
        public static string _publicKey { get; private set; }
        private static string _privatePublicKeys { get; set; }
        public static string _API_PublicKey { get; set; }

        private static byte[] RSA_EncryptStringToBytes(byte[] decrypted, string key) {
            byte[] encrypted;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()) {
                rsa.FromXmlString(key);
                encrypted = rsa.Encrypt(decrypted, false);
            }
            return encrypted;
        }
        private static string RSA_Encrypt(string plainText, string key) {
            byte[] encrypted = RSA_EncryptStringToBytes(Encoding.UTF8.GetBytes(plainText), key);
            return Convert.ToBase64String(encrypted);
        }
        /// <summary>
        /// Шифрує пароль об'єкта user, що наслідує інтерфейс IUser
        /// </summary>
        /// <param name="user">Об'єкт, що наслідує інтерфейс IUser, пароль якого шифрується</param>
        /// <param name="key"></param>
        public static void RSA_Encrypt_IUser(this IUser user) {
            user.UserPassword = RSA_Encrypt(user.UserPassword, _API_PublicKey);
        }

        private static byte[] RSA_DecryptStringToBytes(byte[] encrypted, string key) {
            byte[] decrypted;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()) {
                rsa.FromXmlString(key);
                decrypted = rsa.Decrypt(encrypted, false);
            }
            return decrypted;
        }
        private static string RSA_Decrypt(string cypher, string key) {
            byte[] plainText = RSA_DecryptStringToBytes(Convert.FromBase64String(cypher), key);
            return Encoding.UTF8.GetString(plainText);
        }
        /// <summary>
        /// Дешифрує пароль об'єкта user, що був зашифрований відкритим ключом наданим API, використовуючи закритий ключ API
        /// </summary>
        /// <param name="user">Об'єкт, що наслідує інтерфейс IUser, пароль якого потрібно дешифрувати</param>
        public static void RSA_Decrypt_IUser(this IUser user) {
            user.UserPassword = RSA_Decrypt(user.UserPassword, _privatePublicKeys);
        }


        private static void RSA_KeysGenerate(out string publicKey, out string privatePublicKeys) {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512)) {
                publicKey = rsa.ToXmlString(false);
                privatePublicKeys = rsa.ToXmlString(true);
            }
        }
        public static void RSA_KeysGenerate_Save() {
            string publicKey, privatePublicKeys;
            RSA_KeysGenerate(out publicKey, out privatePublicKeys);
            _publicKey = publicKey;
            _privatePublicKeys = privatePublicKeys;
        }
    }
}