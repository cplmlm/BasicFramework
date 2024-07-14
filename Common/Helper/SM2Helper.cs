using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;

namespace Common.Helper
{
    public class SM2Helper
    {
        #region 获取公钥私钥
        /// <summary>
        /// 获取公钥私钥
        /// </summary>
        /// <returns></returns>
        public static SM2Model GetKey()
        {
            SM2 sm2 = SM2.Instance;
            AsymmetricCipherKeyPair key = sm2.ecc_key_pair_generator.GenerateKeyPair();
            ECPrivateKeyParameters ecpriv = (ECPrivateKeyParameters)key.Private;
            ECPublicKeyParameters ecpub = (ECPublicKeyParameters)key.Public;
            BigInteger privateKey = ecpriv.D;
            ECPoint publicKey = ecpub.Q;
            SM2Model sM2Model = new SM2Model();
            sM2Model.PrivateKey = Encoding.UTF8.GetString(Hex.Encode(privateKey.ToByteArray())).ToUpper();
            sM2Model.PublicKey = Encoding.UTF8.GetString(Hex.Encode(publicKey.GetEncoded())).ToUpper();
            return sM2Model;
        }
        #endregion

        #region 加密
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="publickey">公钥</param>
        /// <param name="sourceData">需要加密的值</param>
        /// <returns>加密结果</returns>
        public static string Encrypt(string publickey, string sourceData)
        {
            var data = Encrypt(Hex.Decode(publickey), Encoding.UTF8.GetBytes(sourceData));
            return data;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="data">需要加密的值</param>
        /// <returns></returns>
        private static String Encrypt(byte[] publicKey, byte[] data)
        {
            if (null == publicKey || publicKey.Length == 0)
            {
                return null;
            }
            if (data == null || data.Length == 0)
            {
                return null;
            }

            byte[] source = new byte[data.Length];
            Array.Copy(data, 0, source, 0, data.Length);

            Cipher cipher = new Cipher();
            SM2 sm2 = SM2.Instance;

            ECPoint userKey = sm2.ecc_curve.DecodePoint(publicKey);

            ECPoint c1 = cipher.Init_enc(sm2, userKey);
            cipher.Encrypt(source);

            byte[] c3 = new byte[32];
            cipher.Dofinal(c3);

            String sc1 = Encoding.UTF8.GetString(Hex.Encode(c1.GetEncoded()));
            String sc2 = Encoding.UTF8.GetString(Hex.Encode(source));
            String sc3 = Encoding.UTF8.GetString(Hex.Encode(c3));

            return (sc1 + sc2 + sc3).ToUpper();
        }
        #endregion

        #region 解密

        /// <summary>
        /// 
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="encryptedData"></param>
        /// <returns></returns>
        public static string Decrypt(string privateKey, string encryptedData)
        {
            var data = Encoding.UTF8.GetString(Decrypt(Hex.Decode(privateKey), Hex.Decode(encryptedData)));
            return data;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="encryptedData"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] privateKey, byte[] encryptedData)
        {
            if (null == privateKey || privateKey.Length == 0)
            {
                return null;
            }
            if (encryptedData == null || encryptedData.Length == 0)
            {
                return null;
            }

            String data = Encoding.UTF8.GetString(Hex.Encode(encryptedData));

            byte[] c1Bytes = Hex.Decode(Encoding.UTF8.GetBytes(data.Substring(0, 130)));
            int c2Len = encryptedData.Length - 97;
            byte[] c2 = Hex.Decode(Encoding.UTF8.GetBytes(data.Substring(130, 2 * c2Len)));
            byte[] c3 = Hex.Decode(Encoding.UTF8.GetBytes(data.Substring(130 + 2 * c2Len, 64)));

            SM2 sm2 = SM2.Instance;
            BigInteger userD = new BigInteger(1, privateKey);

            //ECPoint c1 = sm2.ecc_curve.DecodePoint(c1Bytes);

            ECPoint c1 = sm2.ecc_curve.DecodePoint(c1Bytes);
            Cipher cipher = new Cipher();
            cipher.Init_dec(userD, c1);
            cipher.Decrypt(c2);
            cipher.Dofinal(c3);

            return c2;
        }
#endregion
    }
}
