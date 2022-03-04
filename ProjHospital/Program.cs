using System;

namespace ProjHospital
{
    internal class Program
    {
        public static Fila fila_normal = new Fila();
        public static Fila fila_preferencial = new Fila();

        public static Paciente CadastrarPaciente(string cpf)
        {
            Console.Clear();

            string sexo;
            Console.WriteLine(" -- CADASTRO DE PACIENTE -- ");
            Console.WriteLine("Digite o CPF: ");
            Console.WriteLine(cpf);
            Console.WriteLine("Digite o Nome: ");
            string nome = Console.ReadLine().ToUpper();
            do
            {
                Console.WriteLine("Digite o sexo [ M (Masculino) / F (Feminino) ]: ");
                sexo = Console.ReadLine().ToUpper();
                if (sexo != "M" && sexo != "F")
                {
                    Console.Clear();
                    Console.WriteLine("Opção inválida, tente novamente");
                }

            } while (sexo != "M" && sexo != "F");

            Console.WriteLine("Digite a data de nascimento do Paciente: ");
            DateTime data = DateTime.Parse(Console.ReadLine());

            Paciente paciente = new Paciente(nome, cpf, sexo, data);

            paciente.SalvarInformacoesPacienteNoArquivo();

            return paciente;
        }

        public static void NovoPaciente()
        {
            Console.Clear();

            Paciente paciente = new Paciente();

            Console.WriteLine(" --  Buscar Paciente -- ");
            Console.WriteLine("Digite o CPF: ");
            string cpf = Console.ReadLine();

            paciente = paciente.BuscarInformacaoPaciente(cpf);

            if (paciente != null)
            {
                Console.WriteLine(" -- Ficha do Paciente -- ");

                Console.WriteLine(paciente.ToString());
            }
            else
            {
                paciente = CadastrarPaciente(cpf);
            }
            if((DateTime.Now.Year - paciente.DataNasc.Year) >= 60){
                
                fila_preferencial.InserirDadosNoArquivo(paciente, "FilaPreferencial");
            }
            else
            {
                fila_normal.InserirDadosNoArquivo(paciente, "FilaNormal");
            }
                
        }

        public static void BuscarPacienteNaFila()
        {
            Console.Clear();

            Paciente paciente = new Paciente();


            Console.WriteLine("Digite o CPF: ");
            string cpf = Console.ReadLine();

          
            if ((paciente = fila_normal.Buscar(cpf)) != null)
            {
                Console.WriteLine(paciente.ToString());
                Console.WriteLine("Aguardando na fila normal");
            }
            else if((paciente = fila_preferencial.Buscar(cpf)) != null)
            {
                Console.WriteLine(paciente.ToString());
                Console.WriteLine("Aguardando na fila preferencial");
            }    
            else
            {
                Console.WriteLine("Paciente não está em nenhuma fila");
            }

        }

        public static void ChamarExame(Fila fila, string arquivo)
        {
            Console.Clear();

            Console.WriteLine(fila.Head.ToString());

            string opcao;

            do
            {
                Console.WriteLine("\nQual o resultado do teste de Covid-19?");
                Console.WriteLine("[1] Positivo");
                Console.WriteLine("[2] Negativo");
                Console.WriteLine("[3] Não Reagente");
                opcao = Console.ReadLine();

            } while (opcao != "1" && opcao != "2" && opcao != "3");

            string resultadoTeste;
                

            if(opcao == "1")
            {
                resultadoTeste = "POSITIVO";
            }else if(opcao == "2")
            {
                resultadoTeste = "NEGATIVO";
            }
            else
            {
                resultadoTeste = "NÃO REAGENTE";
            }

            Console.WriteLine("\nQuantidade em dias com os sintomas: ");
            int dias = int.Parse(Console.ReadLine());


            string[] sintomas = Sintomas();

            string[] comorbidades = Comorbidade();


            Console.Clear();

            Console.WriteLine(fila.Head.ToString());

            Console.WriteLine($"\nResultado teste de Covid: {resultadoTeste}");

            Console.WriteLine("\n[Sintomas]");
            Console.WriteLine($"Febre: {sintomas[0]} \n" +
                $"Dor de Cabeça: {sintomas[1]}\n" +
                $"Falta de Paladar: {sintomas[2]}\n" +
                $"Falta de Olfato:  {sintomas[3]}");
            Console.WriteLine($"\nQuantidade de dias com sintomas: {dias}");


            Console.Write("\n[Comorbidades] ");

            if (comorbidades[0] == null)
                Console.WriteLine("Nenhuma");
            else
            {
                Console.WriteLine();
                foreach (string comorbidade in comorbidades)
                    if (comorbidade != null)
                        Console.WriteLine(comorbidade);
            }

            string acao;

            do
            {
                Console.WriteLine($"\nO que deseja fazer com o paciente {fila.Head.Nome}? ");
                Console.WriteLine("[1] Dar alta");
                Console.WriteLine("[2] Colocar em quarentena");
                Console.WriteLine("[3] Mandar para emergência");

                acao = Console.ReadLine();

                switch (acao)
                {
                    case "1":
                        fila.Head.SalvarHistoricoDoPaciente(fila.Head.CPF, resultadoTeste, sintomas, dias, comorbidades, "Liberado");
                        fila.RemoverDadosDoArquivo(fila, arquivo);
                        break;

                    case "2":
                        fila.Head.SalvarHistoricoDoPaciente(fila.Head.CPF, resultadoTeste, sintomas, dias, comorbidades, "Em Quarentena");
                        fila.RemoverDadosDoArquivo(fila, arquivo);
                        break;
                    case "3":
                        break;

                    default:
                        Console.WriteLine("Ação inválida");
                        break;
                }

            } while (acao != "1" && acao != "2" && acao != "3");
        }

        public static void BuscarHistorico()
        {
            Console.Clear();

            Paciente paciente = new Paciente();

            Console.WriteLine("****************** Buscar Paciente ****************");
            Console.Write("\nInforme o CPF: ");
            string cpf = Console.ReadLine();
            Console.WriteLine("\n***************************************************");

            paciente = paciente.BuscarInformacaoPaciente(cpf);

            if (paciente != null)
            {
                Console.Clear();

                Console.WriteLine("******************** Ficha do Paciente ******************");
                Console.WriteLine(paciente.ToString());
                Console.WriteLine("\n****************** Histórico do Paciente ****************");
                paciente.CarregarHistoricoDoPaciente(cpf);
                Console.WriteLine("\n*********************************************************");
            }
            else
                Console.WriteLine("Paciente não encontrado");

        }

        private static string[] Sintomas()
        {
            string[] sintomas = new string[4] { "NÃO", "NÃO", "NÃO", "NÃO" };

            string febre, dorCabeca, semPaladar, semOlfato;


            do
            {
                Console.WriteLine("Esta/teve com febre? [S - SIM] [N - NÃO]");
                febre = Console.ReadLine().ToUpper();
                if (febre == "S")
                {
                    sintomas[0] = "SIM";
                }
                else if (febre == "N")
                {
                    sintomas[0] = "NÃO";
                }
                else
                {
                    Console.WriteLine("Opção inválida!!!");
                }
            } while (febre != "S" && febre != "N");
            do
            {
                Console.WriteLine("Está ou esteve com dor de cabeça? [S - SIM] [N - NÃO]");
                dorCabeca = Console.ReadLine().ToUpper();
                if (dorCabeca == "S")
                {
                    sintomas[1] = "SIM";
                }
                else if (dorCabeca == "N")
                {
                    sintomas[1] = "NÃO";
                }
                else
                {
                    Console.WriteLine("Opção inválida!");
                }
            } while (dorCabeca != "S" && dorCabeca != "N");
            do
            {
                Console.WriteLine("Está ou esteve com dor de cabeça? [S - SIM] [N - NÃO]");
                semPaladar = Console.ReadLine().ToUpper();
                if (semPaladar == "S")
                {
                    sintomas[2] = "SIM";
                }
                else if (semPaladar == "N")
                {
                    sintomas[2] = "NÃO";
                }
                else
                {
                    Console.WriteLine("Opção inválida!");
                }
            } while (semPaladar != "S" && semPaladar != "N");
            do
            {
                Console.WriteLine("Está ou esteve com dor de cabeça? [S - SIM] [N - NÃO]");
                semOlfato = Console.ReadLine().ToUpper();
                if (semOlfato == "S")
                {
                    sintomas[3] = "SIM";
                }
                else if (semOlfato == "N")
                {
                    sintomas[3] = "NÃO";
                }
                else
                {
                    Console.WriteLine("Opção inválida!");
                }

            } while (semOlfato != "S" && semOlfato != "N");

            return sintomas;
        }

        public static string[] Comorbidade()
        {
            string[] comorbidadeArray = new string[5] { null, null, null, null, null};
            Console.WriteLine("Possui comorbidade? [S - SIM] [N - NÃO]");
            string comorbidade = Console.ReadLine().ToUpper();
            int i = 1, x = 0;

            do
            {
                if (comorbidade == "N")
                {
                    break;
                }
                do
                {
                    if (comorbidade != "S" && comorbidade != "N")
                    {
                        Console.WriteLine("Opção inválida, tente novamente!");
                        Console.WriteLine("Possui comorbidade? [S - SIM] [N - NÃO]");
                        comorbidade = Console.ReadLine().ToUpper();
                    }
                } while (comorbidade != "S" && comorbidade != "N");
                if (comorbidade == "S")
                {
                    Console.Write($"Informe a comorbidade {i}: ");
                    comorbidadeArray[x] = Console.ReadLine();
                    Console.WriteLine("Possui mais alguma comorbidade? [S - SIM] [N - NÃO]");
                    comorbidade = Console.ReadLine().ToUpper();
                    x++;
                    i++;
                }
            } while (comorbidade != "N");

            return comorbidadeArray;
        }


        static void Main(string[] args)
        {
            fila_normal.CarregarDadosDoArquivo(fila_normal, "FilaNormal");
            fila_preferencial.CarregarDadosDoArquivo(fila_preferencial, "FilaPreferencial");


            int preferencial = 0;
            int Senhas = 0;
            int opc = 0;

            while (opc != 7)
            {
                Console.Clear();
                Console.WriteLine("Digite uma opção: ");
                Console.WriteLine("1 - Chamar Proximo Paciente");
                Console.WriteLine("2 - Chamar Paciente para Exame: ");
                Console.WriteLine("3 - Buscar Paciente na Fila: ");
                Console.WriteLine("4 - Buscar Histórico do Paciente: ");
                Console.WriteLine("5 - Visualizar Paciente na fila normal: ");
                Console.WriteLine("6 - Visualizar Paciente na fila preferencial: ");
                Console.WriteLine("7 - Sair ");
                opc = Convert.ToInt32(Console.ReadLine());


                switch (opc)
                {
                    case 1:
                        Console.Clear();
                        NovoPaciente();
                        Senhas++;
                        break;
                    case 2:
                        Console.Clear();
                        if (fila_preferencial.Elementos > 0 && preferencial < 2)
                        {
                            Console.WriteLine(fila_preferencial.Head.ToString());
                            preferencial++;
                            ChamarExame(fila_preferencial, "FilaPreferencial");
                        }
                        else if (fila_normal.Elementos > 0)
                        {
                            Console.WriteLine(fila_normal.Head.ToString());
                            preferencial = 0;
                            ChamarExame(fila_normal, "FilaNormal");
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("As Filas estão vazias");
                            preferencial = 0;
                        }       
                        break;
                    case 3:
                        Console.Clear();
                        BuscarPacienteNaFila();
                        break;
                    case 4:
                        Console.Clear();
                        BuscarHistorico();
                        break;
                    case 5:
                        Console.Clear();
                        Console.WriteLine(" -- FILA NORMAL -- ");
                        fila_normal.Imprimir();
                        break;
                    case 6:
                        Console.Clear();
                        Console.WriteLine(" -- FILA PREFERENCIAL -- ");
                        fila_preferencial.Imprimir();
                        break;
                    default:
                        
                        break;
                }

                Console.ReadKey();  
            }
        }
    }
}
