using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServicoTransferenciaArquivo.Library.Tests
{
    [TestClass]
    public class ValidaAcessoTest
    {
        [TestMethod]
        public void AcessoValidoTest()
        {
            var ticks = DateTime.Now.Ticks.ToString();
            var encrypted = RSAEncryption.Encrypt(ticks, false);
            Assert.IsTrue(ValidaAcesso.ValidarTicks(encrypted));
        }

        [TestMethod]
        public void MultiploAcessoValidoTest()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(500);
                var ticks = DateTime.Now.AddSeconds(-29).Ticks.ToString();
                var encrypted = RSAEncryption.Encrypt(ticks, false);
                Assert.IsTrue(ValidaAcesso.ValidarTicks(encrypted));
            }
        }

        [TestMethod]
        public void TempoEsgotadoTest()
        {
            var ticks = DateTime.Now.AddSeconds(-31).Ticks.ToString();
            var encrypted = RSAEncryption.Encrypt(ticks, false);
            Assert.IsFalse(ValidaAcesso.ValidarTicks(encrypted));
        }

        [TestMethod]
        public void TicksRepetidosTest()
        {
            var ticks = DateTime.Now.Ticks.ToString();
            var encrypted = RSAEncryption.Encrypt(ticks, false);
            Assert.IsTrue(ValidaAcesso.ValidarTicks(encrypted));
            Assert.IsFalse(ValidaAcesso.ValidarTicks(encrypted));
        }

        [TestMethod]
        public void TicksNullTest()
        {
            Assert.IsFalse(ValidaAcesso.ValidarTicks(null));
            Assert.IsFalse(ValidaAcesso.ValidarTicks(string.Empty));
            Assert.IsFalse(ValidaAcesso.ValidarTicks("  "));
        }

        [TestMethod]
        public void TicksInvalidoTest()
        {
            Assert.IsFalse(ValidaAcesso.ValidarTicks(DateTime.Now.Ticks.ToString()));
        }
    }
}
