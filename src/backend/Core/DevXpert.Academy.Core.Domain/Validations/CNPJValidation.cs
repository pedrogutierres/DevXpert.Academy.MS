namespace DevXpert.Academy.Core.Domain.Validations
{
    public class CNPJValidation
    {
        public static bool Validar(string cnpj) => Validar(cnpj, out _);
        public static bool Validar(string cnpj, out string msgDeErro)
        {
            if (string.IsNullOrEmpty(cnpj))
            {
                msgDeErro = "O CNPJ não foi preenchido.";
                return false;
            }

            if (cnpj.Length > 14)
            {
                msgDeErro = "O CNPJ deve conter no máximo 14 caracteres.";
                return false;
            }

            while (cnpj.Length != 14)
                cnpj = '0' + cnpj;

            var igual = true;
            var apenasNumeros = true;
            for (var i = 1; i < 14 && igual; i++)
            {
                if (cnpj[i] != cnpj[0])
                {
                    igual = false;
                }

                if (!char.IsNumber(cnpj[i]))
                    apenasNumeros = false;
            }

            if (igual || !apenasNumeros || cnpj == "12345678901234")
            {
                msgDeErro = "O CNPJ não pode ser igual a uma sequência identica ou incremental.";
                return false;
            }

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
            {
                msgDeErro = "O CNPJ deve ser conter apenas 14 números.";
                return false;
            }
            var tempCnpj = cnpj.Substring(0, 12);
            var soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            var resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            var digito = resto.ToString();
            tempCnpj += digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito += resto.ToString();

            if (!cnpj.EndsWith(digito))
            {
                msgDeErro = "CNPJ inválido.";
                return false;
            }

            msgDeErro = null;
            return true;
        }
    }
}
