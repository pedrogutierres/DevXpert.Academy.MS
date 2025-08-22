using System;
using System.Linq;

namespace DevXpert.Academy.Financeiro.AntiCorruption
{
    public class PayPalGateway : IPayPalGateway
    {
        public string GetCardHashKey(string serviceKey, string cartaoCredito)
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public string GetPayPalServiceKey(string apiKey, string encriptionKey)
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public bool CommitTransaction(string cardHashKey, string orderId, decimal amount)
        {
            if (orderId == "-1") // Apenas exemplo, simula erro de transação
                return false;

            return true;
        }

        public bool RollbackTransaction(string orderId)
        {
            if (orderId == "-1") // Apenas exemplo, simula erro de transação
                return false;

            return true;
        }
    }
}