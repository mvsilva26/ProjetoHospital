using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjHospital
{
    internal class Fila
    {

        
        public Paciente Head { get; set; }
        public Paciente Tail { get; set; }
        public int Elementos { get; set; }
        public string PathFile { get; set; }

        public Fila()
        {
            Head = null;
            Tail = null;
            Elementos = 0;
            PathFile = DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "_");
        }

        public void Imprimir()
        {
            if(Elementos == 0)
            {
                Console.WriteLine("[A fila está vazia]");
                return;
            }

            Paciente paciente = Head;

            do
            {
                Console.WriteLine(paciente.ToString());
                Console.WriteLine("\n -- ");
                paciente = paciente.Proximo;
            } while (paciente != null);
        }

        public void Inserir(Paciente paciente)
        {
            if (vazia())
            {
                Head = paciente;
                Tail = paciente;
            }
            else
            {
                        
                Tail.Proximo = paciente;
                paciente.Anterior = Tail;
                Tail = paciente;
                
                
            }
            Elementos++;
            
        }

        public Paciente Buscar(string cpf)
        {
            if(Elementos == 0)
            {
                return null;
            }

            Paciente paciente = Head;

            do
            {
                if (cpf == paciente.CPF)
                {
                    return paciente;
                }

                paciente = paciente.Proximo;

            } while (paciente != null);

            return null;

        }

        public void Remover(string cpf)
        {
            if (Elementos == 0)
            {
                return;
            }
            if(Head.CPF == cpf)
            {
                Head = Head.Proximo;
                Elementos--;
            }
            if(Head == null)
            {
                Tail = null;
            }
            
        }




        public void InserirDadosNoArquivo(Paciente paciente, string arquivo)
        {
            bool aguardandoNaFila = false;

            try
            {
                StreamReader sr = new StreamReader($"{PathFile}\\{arquivo}.txt");

                string line = sr.ReadLine();

                while (line != null)
                {
                    string[] dados = line.Split(";");

                    if (paciente.CPF == dados[0])
                    {
                        aguardandoNaFila = true;
                        sr.Close();
                        break;
                    }

                    line = sr.ReadLine();
                }
                sr.Close();

                if (!aguardandoNaFila)
                {
                    StreamWriter sw = new StreamWriter($"{PathFile}\\{arquivo}.txt", append: true);
                    sw.WriteLine($"{paciente.CPF};{paciente.Nome};{paciente.Sexo};{paciente.DataNasc.ToString("dd/MM/yyyy")};");
                    sw.Close();

                    Inserir(paciente);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
            
        }

        public void CarregarDadosDoArquivo(Fila fila_paciente, string arquivo)
        {
            bool arquivoFila = false;
            bool diretorioFila = false;

            try
            {
                if (Directory.Exists(PathFile))
                {
                    diretorioFila = true;
                }
                else
                {
                    Directory.CreateDirectory(PathFile);
                    diretorioFila = true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }

            if (diretorioFila)
            {
                try
                {
                    if (File.Exists($"{PathFile}\\{arquivo}.txt"))
                    {
                        arquivoFila = true;
                    }
                    else
                    {
                        File.Create($"{PathFile}\\{arquivo}.txt").Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                }

                if (arquivoFila)
                {

                    try
                    {
                        StreamReader sr = new StreamReader($"{PathFile}\\{arquivo}.txt");

                        string line = sr.ReadLine();

                        while (line != null)
                        {
                            string[] dados = line.Split(";");

                            Paciente paciente = new Paciente(dados[1], dados[0], dados[2], DateTime.Parse(dados[3]));

                            fila_paciente.Inserir(paciente);

                            line = sr.ReadLine();
                        }

                        sr.Close();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.ToString());
                    }
                }
            }
            
            

        }

        public void RemoverDadosDoArquivo(Fila fila, string arquivo)
        {
            try
            {
                StreamWriter sw = new StreamWriter($"{PathFile}\\{arquivo}.txt");

                fila.Remover(fila.Head.CPF);

                for (Paciente paciente = Head; paciente != null; paciente = paciente.Proximo)
                {
                    sw.WriteLine($"{paciente.CPF};{paciente.Nome};{paciente.DataNasc.ToString("dd/MM/yyyy")};{paciente.Sexo};");
                }
    
                sw.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }




        public bool vazia()
        {
            if(Head == null && Tail == null)
            {
                return true;
            }
            else
            {
                return false;   
            }
        }


    }
}
