using System;
using System.Net.Mail;

namespace WebApp.Identity.Email
{
    public static class EnviarEmail
    {

        private static void enviar(string corpo, string title)
        {
            try
            {
                MailMessage Mail = new MailMessage();
                //Digite aqui o servidor SMTP para a aplicação acessar
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                //A porta de acesso
                int port = 587;

                string MeuEmail = "v.vieira.go@gmail.com";
                string MeuPassword = "@Email$702220";

                //Digite aqui o e-mail que enviara a mensagem e o nome mostrado(remetente)
                Mail.From = new MailAddress(MeuEmail, "Vinícius");
                //Digite o e-amil da pessoa que vai recebe (destinatario)
                Mail.To.Add("vinicius.abreu@lg.com.br");
                //Nivel de prioridade da mensagem
                Mail.Priority = MailPriority.High;
                //Assunto da mensagem
                Mail.Subject = "TESTE";

                //Corpo da página com o recurso html ligado.
                Mail.Body = corpoDaMensagem(title, corpo);

                //Permite que seja inserido uma estrutura html na mensagem.
                Mail.IsBodyHtml = true;

                //Anexa arquivo na mensagem
                //if (txtAnexo.Text != "") Mail.Attachments.Add(new Attachment(txtAnexo.Text));

                //Porta utilizada no servidor SMTP
                SmtpServer.Port = port;

                //Digite aqui a credencial de e-mail e senha válidos para acessar.
                SmtpServer.Credentials = new System.Net.NetworkCredential(MeuEmail, MeuPassword);
                //A autenticação de segura do e-mail - SSL
                SmtpServer.EnableSsl = true;
                //SmtpServer.Timeout = 10000;
                SmtpServer.Send(Mail);

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Erro ao enviar e-mail: {0}", ex));
            }

        }

        /// <summary>
        /// Cria o corpo da página.
        /// </summary>
        /// <param name="nome">Recebe o nome do destinatário.</param>
        /// <param name="mensagem">Recebe uma mensagem escrita.</param>
        /// <returns>Retorna o corpo com as informações inseridas.</returns>
        public static string corpoDaMensagem(string nome, string mensagem)
        {
            string a = "c1.staticflickr.com/5/4185/34282917092_d57050bf2c_b.jpg";
            string body = "<div style=\"font-family: Arial, Helvetica, sans-serif;\">" +
                                "E-mail de teste do 'Programa para enviar e-mail' em c#<br/><br/>" +
                                "<table width=\"429\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border: 2px solid #09F;text-align:center;\">" +
                                    "<tr>" +
                                        "<td width=\"397\" height=\"93\" style=\"border: 2px solid #09F; font-size:20px;color: " +
                                        "#FFF; background:#09F\"><b> Nome do contato:</b> " + nome + "</td>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td style=\"border: 2px solid #09F; padding:10px; text-align:left;\"><p><b>Mensagem:</b></p>" +
                                            "<p>" + mensagem + "</p></td>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td style=\"border: 2px solid #09F; background:#0CF; padding:10px;\"><p><b>Imagem modelo:</b></p>" +
                                            "<p><img src=\"http://" + a + " \" width=\"313\" height=\"204\" /></p></td>" +
                                    "</tr>" +
                                "</table>" +
                            "</div>";
            return body;
        }
    }
}