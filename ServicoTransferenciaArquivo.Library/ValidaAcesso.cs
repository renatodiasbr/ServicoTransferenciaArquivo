using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicoTransferenciaArquivo.Library
{
    public class ValidaAcesso
    {
        public static bool ValidarTicks(string ticks)
        {
            if (string.IsNullOrWhiteSpace(ticks))
            {
                return false;
            }

            try
            {
                var decryptedTicks = RSAEncryption.Decrypt(ticks, false);
                var binaryTicks = Convert.ToInt64(decryptedTicks);
                var dateTicks = DateTime.FromBinary(binaryTicks);

                if ((DateTime.Now - dateTicks).TotalSeconds > TIMEOUT_SECONDS)
                {
                    return false;
                }
                else
                {
                    if (VerificarTicksValidado(binaryTicks))
                    {
                        return false;
                    }

                    AdicionarTicksAutorizado(binaryTicks);
                    LimparTicksObsoletos();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static void AdicionarTicksAutorizado(long ticks)
        {
            allTicks.Add(ticks);
        }

        private static void LimparTicksObsoletos()
        {
            var activeTicks = allTicks.Where(e => (DateTime.Now - DateTime.FromBinary(e)).TotalSeconds < TIMEOUT_SECONDS).ToList();
            allTicks = activeTicks;
        }

        private static bool VerificarTicksValidado(long ticks)
        {
            return allTicks.Contains(ticks);
        }

        private static List<long> allTicks = new List<long>();
        private static int TIMEOUT_SECONDS = 30;
    }
}
