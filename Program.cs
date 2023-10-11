using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using MySql.Data.MySqlClient;

public class Veiculo
{
    public string Placa { get; set; }
    public string Ticket { get; set; }
    public DateTime HorarioEntrada { get; set; }
    public DateTime HorarioSaida { get; set; }
    public bool PagouFracao30Minutos { get; set; }
    public bool PagouAlgumPeriodo { get; set; }
    public bool PagouPeriodoTotal
    {
        get
        {
            TimeSpan duracao = HorarioSaida - HorarioEntrada;
            if (duracao.TotalMinutes <= 30)
            {
                return true;
            }
            else
            {
                return PagouAlgumPeriodo;
            }
        }
    }

}


public class Estacionamento
{
    private int capacidadeestacionamento;

    private bool VeiculoEstaEstacionado(string placa)
    {
        return ListaPatio.Exists(v => v.Placa == placa);
    }
    public int Capacidade
    {
        get { return capacidadeestacionamento; }
        set
        {
            capacidadeestacionamento = value;
        }
    }
  

    public decimal ValorFracao { get; set; }
    public List<Veiculo> ListaPatio { get; set; }

    public int VagasDisponiveis
    {
        get
        {
            int veiculosEstacionados = ListaPatio.Count(predicate: v =>
            {
                return v.HorarioSaida
                       == null;
            });
            return Capacidade - veiculosEstacionados;
        }
    }


    public string Ticket { get; set; }
    private string senhaAdministrador;
    private Dictionary<TimeSpan, decimal> tabelaValores;
    private object set;
    private readonly string? senha;

    public int VeiculosEstacionados
    {
        get { return ListaPatio.Count; }
    }

    public static void Main()
    {
        // Conecte-se ao banco de dados
        using (var connection = CriarConexao())
        {
            connection.Open();

            // Execute uma consulta para obter as informações do banco de dados
            string sql = "SELECT capacidade, senha FROM configuracao_estacionamento WHERE id_estacionamento = 1";
            using (var command = new MySqlCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Recupere os valores do banco de dados
                        int capacidade = Convert.ToInt32(reader["capacidade"]);
                        string senha = reader["senha"].ToString();

                        // Crie a instância do objeto Estacionamento com as informações obtidas do banco de dados
                        Estacionamento estacionamento = new Estacionamento(capacidade, senha);

                        Console.OutputEncoding = System.Text.Encoding.UTF8;

                        ExecutarEstacionamento(estacionamento);

                        // estacionamento.AtualizarCapacidade();

                        // estacionamento.ExibirCapacidade();
                    }
                }
            }
        }
    }


    public List<string> HistoricoVeiculo(string placa)
    {
        List<string> historico = new List<string>();

        string connectionString = "server=localhost;user=rsouza;password=Losefdsor1!;database=projeto_estacionamento;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Exemplo de consulta SQL para obter o histórico do veículo com base na placa
            string sql = $"SELECT * FROM historico_veiculos WHERE placa = '{placa}'";
            using (var command = new MySqlCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Ler os dados do histórico do veículo
                        string dataAcesso = reader.IsDBNull("data_acesso") ? " " : reader.GetDateTime("data_acesso").ToString();
                        string horarioEntrada = reader.GetDateTime("horario_entrada").ToString();
                        string horarioSaida = reader.IsDBNull("horario_saida") ? " " : reader.GetDateTime("horario_saida").ToString();
                        decimal valorPago = reader.IsDBNull("valor_pago") ? 0.0m : reader.GetDecimal("valor_pago");


                        // Formatar as informações do histórico
                        string historicoVeiculo = $"Data de Acesso: {dataAcesso} - Horário de Entrada: {horarioEntrada}";

                        if (!string.IsNullOrEmpty(horarioSaida))
                        {
                            historicoVeiculo += $" - Horário de Saída: {horarioSaida} - Valor Pago: R${valorPago}";
                        }
                        else
                        {
                            historicoVeiculo += " - Veículo ainda estacionado";
                        }

                        historico.Add(historicoVeiculo);

                        Console.Clear();
                    }
                }
            }
        }

        return historico;
    }



    public Estacionamento(int capacidade, decimal valorFracao, string senha)
    {
        Capacidade = capacidade;
        ValorFracao = valorFracao;
        senhaAdministrador = senha;
        ListaPatio = new List<Veiculo>();
        tabelaValores = new Dictionary<TimeSpan, decimal>();
        //Private VeiculosEstacionados = Capacidade - VagasDisponiveis; 



    }

    public Estacionamento(int capacidade, string? senha)
    {
        Capacidade = capacidade;
        this.senha = senha;
    }

    public bool AutenticarAdministrador(string senha)
    {
        return senha == senhaAdministrador;
    }

    public void DefinirCapacidade(int capacidade)
    {
        if (Capacidade == capacidade)
        {
            // Console.WriteLine($"A capacidade já está definida como {capacidade} vagas.");
            return;
        }

        // Cria uma conexão com o banco de dados
        using (var connection = CriarConexao())
        {
            // Abre a conexão
            connection.Open();

            // Cria um comando para atualizar a capacidade do estacionamento
            string sql = "UPDATE configuracao_estacionamento SET capacidade = @capacidade WHERE id_estacionamento = 1";
            using (var command = new MySqlCommand(sql, connection))
            {
                // Adiciona o valor da nova capacidade ao comando
                command.Parameters.AddWithValue("@capacidade", capacidade);

                // Executa o comando
                command.ExecuteNonQuery();
            }

            // Fecha a conexão
            connection.Close();
        }

        // Atualiza a propriedade Capacidade com o novo valor
        Capacidade = capacidade;

        // Exibe uma mensagem na saída padrão
        Console.WriteLine($"Capacidade do estacionamento definida para {capacidade} vagas.");
    }








    public void ExibirCapacidade()
    {
        using (var connection = CriarConexao())
        {
            connection.Open();

            string sql = "SELECT capacidade FROM configuracao_estacionamento WHERE id_estacionamento = 1";
            using (var command = new MySqlCommand(sql, connection))
            {
                var result = command.ExecuteScalar();
                int capacidade = Convert.ToInt32(result);
                int vagasDisponiveis = capacidade - GetNumeroVeiculosEstacionados();

                Console.WriteLine($"Capacidade do estacionamento: {capacidade} vagas");
                Console.WriteLine($"Vagas disponíveis: {vagasDisponiveis}");
            }
        }


        int GetNumeroVeiculosEstacionados()
        {
            using (var connection = CriarConexao())
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM historico_veiculos WHERE horario_saida IS NULL";
                using (var command = new MySqlCommand(sql, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
    }


    //public void SalvarCapacidadeEmArquivo(string caminhoArquivo, Estacionamento estacionamento)
    //{
      //  int vagasDisponiveis = estacionamento.VagasDisponiveis; // Obtenha as vagas disponíveis

        //string conteudoArquivo = $"Capacidade atual: {vagasDisponiveis} vagas disponíveis";

        //File.WriteAllText(caminhoArquivo, conteudoArquivo);

//        Console.WriteLine("Capacidade atual salva em arquivo com sucesso.");
  //  }





    public void AdicionarValorTabela(int intervaloMinutos, decimal valor)
    {
        TimeSpan intervalo = TimeSpan.FromMinutes(intervaloMinutos);
        TimeSpan intervaloHoras = TimeSpan.FromHours(intervalo.TotalHours);

        using (var connection = CriarConexao())
        {
            connection.Open();

            string sql = "INSERT INTO tabela_valores (intervalo, valor) VALUES (@intervalo, @valor)";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@intervalo", intervaloMinutos);
                command.Parameters.AddWithValue("@valor", valor);
                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine($"Valor {valor} adicionado para o intervalo de {intervaloHoras.TotalHours} horas.");
    }



    public void Pernoite(int intervaloMinutos, decimal valor)
    {
        if (intervaloMinutos > 24)
        {
            valor = 99;
        }

        TimeSpan intervalo = TimeSpan.FromMinutes(intervaloMinutos);
        TimeSpan intervaloHoras = TimeSpan.FromHours(intervalo.TotalHours);

        using (var connection = CriarConexao())
        {
            connection.Open();

            string sql = "INSERT INTO tabela_valores (intervalo, valor) VALUES (@intervalo, @valor)";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@intervalo", intervaloMinutos);
                command.Parameters.AddWithValue("@valor", valor);
                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine($"Valor {valor} adicionado para o intervalo de {intervaloHoras.TotalHours} horas.");
    }

    public void Pernoite(int intervaloMinutos)
    {
        Pernoite(intervaloMinutos, ValorFracao);
    }

    public void DefinirValorFracao(decimal valor)
    {
        ValorFracao = valor;
        Console.WriteLine($"Valor da fração definido como {valor}.");
    }

   

    public void ConsultarValores()
    {
        using (var connection = CriarConexao())
        {
            connection.Open();

            string sql = "SELECT SEC_TO_TIME(intervalo * 60) AS intervalo_formatado, valor FROM tabela_valores";
            using (var command = new MySqlCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("Tabela de Valores:");
                    while (reader.Read())
                    {
                        string intervaloFormatado = reader.GetString("intervalo_formatado");
                        decimal valor = reader.GetDecimal("valor");
                        Console.WriteLine($"{intervaloFormatado} - R$ {valor}");


                    }
                }
            }
        }
    }



    public void ExibirListaPatio()
    {
        using (var connection = CriarConexao())
        {
            connection.Open();

            string sql = "SELECT * FROM historico_veiculos WHERE horario_saida IS NULL";

            using (var command = new MySqlCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("\n=== Lista de Veículos no Pátio ===");

                    while (reader.Read())
                    {
                        string placa = reader.GetString("placa");
                        DateTime horarioEntrada = reader.GetDateTime("horario_entrada");
                        string Ticket = reader.GetString("ticket");

                        Console.WriteLine($"Placa: {placa} - Horário de Entrada: {horarioEntrada}- ticket:{Ticket}");
                    }
                }
            }
        }
    }






    public int GetVagasDisponiveis()
    {
        return VagasDisponiveis;
    }


    public void RegistrarEntrada(Veiculo veiculo)
{
    try
    {
        if (VeiculoEstaEstacionado(GetPlaca(veiculo)))
        {
            Console.WriteLine($"O veículo com placa {veiculo.Placa} já está dentro do estacionamento.");
            return;
        }

        int vagasDisponiveis = VagasDisponiveis;
        if (vagasDisponiveis > 0)
        {
            string ticket = GerarNumeroTicket(veiculo);
            veiculo.Ticket = ticket;
            DateTime horarioEntrada = DateTime.Now;
            veiculo.HorarioEntrada = horarioEntrada;

            using (var connection = CriarConexao())
            {
                connection.Open();
                string sql = $"INSERT INTO historico_veiculos (placa, horario_entrada, ticket) VALUES ('{veiculo.Placa}', '{horarioEntrada.ToString("yyyy-MM-dd HH:mm:ss")}', '{ticket}')";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            ListaPatio.Add(veiculo);

            Console.WriteLine($"Veículo com placa {veiculo.Placa} registrado com sucesso! Ticket: {ticket} " +
                $"Data de entrada: {horarioEntrada.ToShortDateString()} " +
                $"Horário de entrada: {horarioEntrada.ToShortTimeString()}\n");

            // Não é necessário atualizar a propriedade VagasDisponiveis
        }
        else
        {
            Console.WriteLine("Não há vagas disponíveis no momento.");
        }
    }
    catch (Exception ex)
    {
        // Tratar a exceção aqui
        Console.WriteLine($"Ocorreu um erro ao registrar a entrada do veículo: {ex.Message}");
    }
}



    private static string GetPlaca(Veiculo veiculo)
    {
        return veiculo.Placa;
    }

    private string GerarNumeroTicket(Veiculo veiculo)
    {
        string ticket = $"{veiculo.Placa}_{DateTime.Now:yyyyMMddHHmmss}";
        return ticket;
    }

    public void RegistrarSaida(Veiculo veiculo)
    {
        using (var connection = CriarConexao())
        {
            connection.Open();
            RegistrarSaida(veiculo, connection);
        }
    }

    static private MySqlConnection CriarConexao()
    {
        string connectionString = "server=localhost;user=rsouza;password=Losefdsor1!;database=projeto_estacionamento;";
        return new MySqlConnection(connectionString);
    }

    private void RegistrarSaida(Veiculo veiculo, MySqlConnection connection)
    {
        if (veiculo.PagouPeriodoTotal)
        {
            string horarioSaidaFormatado = veiculo.HorarioSaida.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = $"UPDATE historico_veiculos SET horario_saida = '{horarioSaidaFormatado}', valor_pago = {CalcularValorEstadia(veiculo)} WHERE placa = '{veiculo.Placa}' AND horario_saida IS NULL";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"Veículo com placa {veiculo.Placa} registrado saída.");
        }
        else
        {
            Console.WriteLine($"O veículo com placa {veiculo.Placa} não pagou o período total da estadia.");
        }
    }


    public decimal CalcularValorEstadia(Veiculo veiculo)
    {
        TimeSpan duracao = veiculo.HorarioSaida - veiculo.HorarioEntrada;
        decimal valorEstadia = 0;
        decimal valorFracaoInicial = tabelaValores.ContainsKey(TimeSpan.FromMinutes(30)) ? tabelaValores[TimeSpan.FromMinutes(30)] : ValorFracao;
        decimal valorFracaoAdicional = tabelaValores.ContainsKey(TimeSpan.FromMinutes(15)) ? tabelaValores[TimeSpan.FromMinutes(15)] : 2;

        if (duracao.TotalMinutes <= 30)
        {
            valorEstadia = valorFracaoInicial;
        }
        else if (veiculo.PagouFracao30Minutos)
        {
            decimal valorEstadiaAdicional = ((decimal)Math.Ceiling((duracao.TotalMinutes - 30) / 15) * valorFracaoAdicional);
            valorEstadia = valorEstadiaAdicional;
        }
        else
        {
            decimal valorEstadiaAdicional = ((decimal)Math.Ceiling((duracao.TotalMinutes - 30) / 15) * valorFracaoAdicional);
            valorEstadia = valorFracaoInicial + valorEstadiaAdicional;
        }

        return valorEstadia;
    }





    public decimal TestarEstadia(string placa, TimeSpan duracaoEstadia)
    {
        Veiculo veiculo = ListaPatio.Find(v => v.Placa == placa);

        if (veiculo != null)
        {
            veiculo.HorarioSaida = veiculo.HorarioEntrada.Add(duracaoEstadia);

            Console.WriteLine($"Veículo com placa {veiculo.Placa} pagou algum período? (S/N)");
            string resposta = Console.ReadLine();

            // Verificar se algum período foi pago
            if (resposta.Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                veiculo.PagouAlgumPeriodo = true;
            }
            else
            {
                veiculo.PagouAlgumPeriodo = false;
            }

            decimal valorEstadia = CalcularValorEstadia(veiculo);
            Console.WriteLine($"\nValor estimado da estadia para o veículo com placa {placa} e duração de {duracaoEstadia}: R${valorEstadia}\n");
            return valorEstadia;
        }
        else
        {
            Console.WriteLine("Veículo não encontrado no pátio.");
            return 0;
        }
    }

    public static void LimparConsole()
    {
        Console.WriteLine("Pressione qualquer tecla para continuar...");
        Console.ReadKey();
        Console.Clear();
        // Console.ReadKey();
    }


    public static void ExecutarEstacionamento(Estacionamento estacionamentoParam)
    {

        {

            // Mudar a cor do console para azul

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;

            // Configurar a codificação UTF-8 para a saída do Console
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Estacionamento estacionamento = new Estacionamento(0, 0, "senha");

            while (true)
            {



                Console.WriteLine("\n=== Menu Principal ===\n");

                Console.WriteLine("1. Registrar entrada de veículo");
                Console.WriteLine("2. Registrar saída de veículo");
                Console.WriteLine("3. Exibir lista de veículos no pátio");
                Console.WriteLine("4. Teste de Estadia");
                Console.WriteLine("5. Acesso administrativo");
                Console.WriteLine("6. Pagar estacionamento");
                Console.WriteLine("7. Sair");
                Console.WriteLine("8. Consultar placa");
                Console.Write("\nSelecione uma opção: \n");


                string opcao = Console.ReadLine();

                if (opcao == "1")
                {
                    Console.Write("\nDigite a placa do veículo: ");
                    string placa = Console.ReadLine();

                    Veiculo veiculo = new Veiculo
                    {
                        Placa = placa,
                        HorarioEntrada = DateTime.Now
                    };

                    estacionamento.RegistrarEntrada(veiculo);
                    LimparConsole();
                }
                else if (opcao == "2")
                {
                    Console.Write("\nDigite a placa do veículo: ");
                    string placa = Console.ReadLine();

                    Veiculo veiculo = estacionamento.ListaPatio.Find(v => v.Placa == placa);

                    if (veiculo != null)
                    {
                        veiculo.HorarioSaida = DateTime.Now;

                        // Verifica se a duração é maior que 30 minutos para definir PagouFracao30Minutos como true
                        if ((veiculo.HorarioSaida - veiculo.HorarioEntrada).TotalMinutes > 30)
                        {
                            veiculo.PagouFracao30Minutos = true;
                        }
                        using (var connection = new MySqlConnection("server=localhost;user=rsouza;password=Losefdsor1!;database=projeto_estacionamento"))
                        {
                            connection.Open();

                            estacionamento.RegistrarSaida(veiculo, connection);

                            decimal valorEstadia = estacionamento.CalcularValorEstadia(veiculo);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nVeículo não encontrado no pátio.");
                        LimparConsole();
                    }
                }


                else if (opcao == "3")
                {
                    estacionamento.ExibirListaPatio();
                    LimparConsole();
                }


                else if (opcao == "4")
                {
                    Console.Clear();

                    Console.Write("\nDigite a placa do veículo: ");
                    string placa = Console.ReadLine();
                    Console.Write("\nDigite a duração estimada da estadia (em horas no formato hh:mm): ");
                    string duracaoEstadiaInput = Console.ReadLine();

                    // Extrair as horas e minutos informados pelo usuário
                    int horas = int.Parse(duracaoEstadiaInput.Substring(0, 2));
                    int minutos = int.Parse(duracaoEstadiaInput.Substring(3, 2));

                    // Calcular a duração estimada em minutos
                    int duracaoEstadiaMinutos = (horas * 60) + minutos;

                    TimeSpan duracaoEstadia = TimeSpan.FromMinutes(duracaoEstadiaMinutos);
                    estacionamento.TestarEstadia(placa, duracaoEstadia);

                    LimparConsole();

                }
                else if (opcao == "6")
                {
                    Console.Write("Placa do veículo para pagar: ");
                    string placaPagar = Console.ReadLine();
                    Veiculo veiculoPagar = estacionamento.ListaPatio.Find(v => v.Placa == placaPagar);

                    if (veiculoPagar != null)
                    {
                        decimal valorEstadia = estacionamento.CalcularValorEstadia(veiculoPagar);
                        Console.WriteLine($"Valor a pagar pelo veículo com placa {veiculoPagar.Placa}: R${valorEstadia}");
                        Console.WriteLine("Processo de pagamento concluído.");
                    }
                    else
                    {
                        Console.WriteLine("Veículo não encontrado no pátio.");
                        LimparConsole();
                    }
                }

                else if (opcao == "8")
                {
                    Console.Clear();
                    Console.Write("\nDigite a placa do veículo: ");
                    string placaConsulta = Console.ReadLine();

                    string connectionString = "server=localhost;user=rsouza;password=Losefdsor1!;database=projeto_estacionamento;";
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        List<string> historicoVeiculo = estacionamento.HistoricoVeiculo(placaConsulta);

                        if (historicoVeiculo.Count > 0)
                        {
                            Console.WriteLine("\nHistórico do veículo:");
                            foreach (string registro in historicoVeiculo)
                            {
                                Console.WriteLine(registro);



                            }
                        }

                        else
                        {
                            Console.WriteLine("\nVeículo não encontrado no histórico.");

                        }
                        LimparConsole();
                    }
                }




                else if (opcao == "5")
                {
                    Console.Write("\nDigite a senha: ");
                    string senha = Console.ReadLine();

                    if (estacionamento.AutenticarAdministrador(senha))
                    {
                        while (true)
                        {

                            LimparConsole();

                            Console.WriteLine("\n=== Menu Administrativo ===\n");

                            Console.WriteLine("1. Definir capacidade");
                            Console.WriteLine("2. Definir valor da fração");
                            Console.WriteLine("3. Adicionar valor à tabela de valores");
                            Console.WriteLine("4. Exibir tabela de valores");
                            Console.WriteLine("5. Exibir capacidade");
                            Console.WriteLine("6. Voltar ao Menu Principal");
                            Console.Write("Selecione uma opção: ");
                            string opcaoAdmin = Console.ReadLine();


                            if (opcaoAdmin == "1")
                            {
                                Console.Write("Digite a nova capacidade: ");
                                int capacidade = Convert.ToInt32(Console.ReadLine());
                                estacionamento.DefinirCapacidade(capacidade);
                                // LimparConsole();

                            }

                            else if (opcaoAdmin == "2")
                            {
                                Console.Write("Digite o novo valor da fração: ");
                                decimal valorFracao = Convert.ToDecimal(Console.ReadLine());
                                estacionamento.DefinirValorFracao(valorFracao);
                                LimparConsole();
                            }
                            else if (opcaoAdmin == "3")
                            {
                                Console.Clear();

                                Console.WriteLine("Exemplo: Para adicionar um valor de R$ 16.00 para um intervalo de 60 minutos, digite:");
                                Console.WriteLine("Intervalo em minutos: 60");
                                Console.WriteLine("Valor: 16.00");
                                Console.WriteLine("-------Presione uma tecla para continuar--------");

                                Console.ReadKey();

                                Console.Write("Digite o intervalo em minutos: ");
                                int intervaloMinutos = Convert.ToInt32(Console.ReadLine());
                                Console.Write("Digite o valor: ");
                                decimal valor = Convert.ToDecimal(Console.ReadLine());
                                estacionamento.AdicionarValorTabela(intervaloMinutos, valor);
                            }



                            else if (opcaoAdmin == "4")
                            {
                                Console.Clear();
                                estacionamento.ConsultarValores();

                            }
                            else if (opcaoAdmin == "5")
                            {
                                estacionamento.ExibirCapacidade();
                                // Console.Clear();
                            }
                            else if (opcaoAdmin == "6")

                            {
                                Console.Clear();
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Opção inválida. Tente novamente.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Senha incorreta. Acesso negado.");
                    }
                }
                else if (opcao == "7")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    LimparConsole();

                }
            }
        }
    }
}