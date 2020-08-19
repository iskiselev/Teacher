using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Teacher.Engine.Modules;

namespace Teacher.Engine.Ui
{
    public class ConsoleUi
    {
        public IConsole Console { get; }

        public ConsoleUi(IConsole console)
        {
            Console = console;
        }

        public async Task Run()
        {
            var module = await SelectTrainModule();
            var settings = new TrainingSettings();
            await DisplayModule(module);
            await DisplaySettings(settings);

            await StartQuiz(new Training(settings, module));
        }

        private async Task StartQuiz(Training training)
        {
            bool skipClean = false;
            while (training.Current != null)
            {
                if (skipClean)
                {
                    Console.WriteLine();
                    skipClean = false;
                }
                else
                {
                    Console.Clear();
                }

                string answer;
                do
                {
                    Console.WriteLine($"{training.Current.Question}\t\t\t{ training.CompletedQuestions}/{ training.ExpectedQuestions}");
                    answer = (await Console.ReadLine()).Trim();
                } while (answer == string.Empty);

                if (answer == "#log")
                {
                    Console.WriteLine();
                    PrintLog(training);
                    skipClean = true;
                    continue;
                }

                if (answer.EndsWith("`!"))
                {
                    answer = answer.Substring(0, answer.Length - 2);
                    skipClean = true;
                }
                if (training.RegisterAnswer(answer) == Reaction.Incorrect)
                {
                    Console.WriteLine("Incorrect!");
                    skipClean = true;
                }
            }

            Console.WriteLine("Done!!!");
            Console.WriteLine();
            PrintLog(training);
        }

        private void PrintLog(Training training)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("Log:");
                var log = JsonSerializer.Serialize(training.QuizItems,
                    new JsonSerializerOptions
                    { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                Console.WriteLine(log);

                File.WriteAllText($"{DateTime.Now:yyyyMMddHHmmss}.txt", log);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private async Task<ITrainModule> SelectTrainModule()
        {
            var discoverdModules = typeof(ConsoleUi).Assembly.GetExportedTypes().Where(
                t => t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ITrainModule)) && t.GetTypeInfo()
                    .DeclaredConstructors
                    .Any(c => c.IsPublic && !c.IsStatic && c.GetParameters().Length == 0)).ToList();

            if (discoverdModules.Count > 1)
            {
                Console.WriteLine("Select Module:");
                for (var i = 0; i < discoverdModules.Count; i++)
                {
                    Console.WriteLine($"({i + 1}) {discoverdModules[i].Name}");
                }

                while (true)
                {
                    var value = (await Console.ReadLine()).Trim();
                    if (int.TryParse(value, out int typedValue) && typedValue > 0 &&
                        typedValue <= discoverdModules.Count)
                    {
                        Console.WriteLine();
                        return (ITrainModule)Activator.CreateInstance(discoverdModules[typedValue - 1]);
                    }

                    Console.WriteLine($"Enter value between 1 and {discoverdModules.Count}");
                }
            }

            return (ITrainModule)Activator.CreateInstance(discoverdModules[0]);
        }

        private async Task DisplaySettings(TrainingSettings settings)
        {
            while (true)
            {
                Console.WriteLine($"Trainer settings");
                foreach (var declaredProperty in settings.GetType().GetTypeInfo().DeclaredProperties
                    .Where(p => p.CanRead && p.CanWrite))
                {
                    Console.WriteLine($"{declaredProperty.Name}: {declaredProperty.GetValue(settings)}");
                }

                Console.WriteLine($"Would you like to modify settings (y/n)?");
                while (true)
                {
                    var response = (await Console.ReadLine()).Trim().ToLower();
                    if (response == "y")
                    {
                        await UpdateProperties(settings);
                        break;
                    }
                    else if (response == "n")
                    {
                        goto outer;
                    }
                }
            }

            outer: ;
        }

        private async Task DisplayModule(ITrainModule module)
        {
            while (true)
            {
                Console.WriteLine($"Using module {module.GetType().Name}");
                foreach (var declaredProperty in module.GetType().GetTypeInfo().DeclaredProperties
                    .Where(p => p.CanRead && p.CanWrite))
                {
                    Console.WriteLine($"{declaredProperty.Name}: {declaredProperty.GetValue(module)}");
                }

                Console.WriteLine($"Total questions: {module.CreateQuizzes().Count()}");

                Console.WriteLine($"Would you like to modify module (y/n)?");
                while (true)
                {
                    var response = (await Console.ReadLine()).Trim().ToLower();
                    if (response == "y")
                    {
                        await UpdateProperties(module);
                        break;
                    }
                    else if (response == "n")
                    {
                        goto outer;
                    }
                }
            }

            outer:
            Console.WriteLine();
            /*foreach (var quiz in module.CreateQuizzes())
            {
                Console.WriteLine($"{quiz.Question} | {quiz.CorrectAnswer}");
            }
            Console.WriteLine();*/
        }

        private async Task UpdateProperties(object module)
        {
            foreach (var declaredProperty in module.GetType().GetTypeInfo().DeclaredProperties.Where(p => p.CanRead && p.CanWrite))
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine($"Set {declaredProperty.Name} (default {declaredProperty.GetValue(module)}):");
                        var value = (await Console.ReadLine()).Trim();
                        if (!string.IsNullOrEmpty(value))
                        {
                            var typedValue = Convert.ChangeType(value, declaredProperty.PropertyType);
                            declaredProperty.SetValue(module, typedValue);
                        }

                        break;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid format. Please use correct format");
                    }

                }
            }
            Console.WriteLine();
        }
    }

}
