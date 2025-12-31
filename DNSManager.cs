using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;

namespace DNSManager
{
    class DNSProvider
    {
        public string Name { get; set; } = "";
        public string PrimaryDNS { get; set; } = "";
        public string SecondaryDNS { get; set; } = "";
        public string Description { get; set; } = "";
    }

    class Program
    {
        private static string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dns_manager_log.txt");

        private static List<DNSProvider> dnsProviders = new List<DNSProvider>
        {
            new DNSProvider
            {
                Name = "Cloudflare",
                PrimaryDNS = "1.1.1.1",
                SecondaryDNS = "1.0.0.1",
                Description = "Лучший для игр и стриминга (Roblox, Telegram, Discord, YouTube)"
            },
            new DNSProvider
            {
                Name = "Google DNS",
                PrimaryDNS = "8.8.8.8",
                SecondaryDNS = "8.8.4.4",
                Description = "Надежный и быстрый DNS от Google"
            },
            new DNSProvider
            {
                Name = "Quad9",
                PrimaryDNS = "9.9.9.9",
                SecondaryDNS = "149.112.112.112",
                Description = "С защитой от вредоносных сайтов"
            },
            new DNSProvider
            {
                Name = "OpenDNS",
                PrimaryDNS = "208.67.222.222",
                SecondaryDNS = "208.67.220.220",
                Description = "С фильтрацией контента"
            },
            new DNSProvider
            {
                Name = "AdGuard DNS",
                PrimaryDNS = "94.140.14.14",
                SecondaryDNS = "94.140.15.15",
                Description = "Блокировка рекламы и трекеров"
            },
            new DNSProvider
            {
                Name = "Yandex DNS",
                PrimaryDNS = "77.88.8.8",
                SecondaryDNS = "77.88.8.1",
                Description = "Быстрый DNS от Yandex"
            },
            new DNSProvider
            {
                Name = "CleanBrowsing",
                PrimaryDNS = "185.228.168.9",
                SecondaryDNS = "185.228.169.9",
                Description = "Семейный фильтр, блокировка вредоносных сайтов"
            },
            new DNSProvider
            {
                Name = "Comodo Secure DNS",
                PrimaryDNS = "8.26.56.26",
                SecondaryDNS = "8.20.247.20",
                Description = "Безопасный DNS с защитой от фишинга"
            },
            new DNSProvider
            {
                Name = "Verisign",
                PrimaryDNS = "64.6.64.6",
                SecondaryDNS = "64.6.65.6",
                Description = "Надежный DNS от Verisign"
            },
            new DNSProvider
            {
                Name = "Level3",
                PrimaryDNS = "4.2.2.1",
                SecondaryDNS = "4.2.2.2",
                Description = "Быстрый DNS от Level3"
            },
            new DNSProvider
            {
                Name = "UncensoredDNS",
                PrimaryDNS = "91.239.100.100",
                SecondaryDNS = "89.233.43.71",
                Description = "Без цензуры, открытый DNS"
            },
            new DNSProvider
            {
                Name = "Alternate DNS",
                PrimaryDNS = "76.76.19.19",
                SecondaryDNS = "76.223.122.150",
                Description = "Быстрый альтернативный DNS"
            }
        };

        static void Main(string[] args)
        {
            try
            {
                // Настройка кодировки
                try
                {
                    Console.OutputEncoding = Encoding.UTF8;
                    Console.InputEncoding = Encoding.UTF8;
                }
                catch
                {
                    // Игнорируем ошибки кодировки
                }

                // Проверка прав администратора
                if (!IsRunAsAdministrator())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("========================================");
                    Console.WriteLine("ОШИБКА: Программа должна быть запущена");
                    Console.WriteLine("от имени администратора!");
                    Console.WriteLine("========================================");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("Инструкция:");
                    Console.WriteLine("1. Правой кнопкой мыши на DNSManager.exe");
                    Console.WriteLine("2. Выберите 'Запуск от имени администратора'");
                    Console.WriteLine();
                    Console.WriteLine("Нажмите любую клавишу для выхода...");
                    Console.ReadKey();
                    return;
                }

                WriteLog("========================================");
                WriteLog("DNS Manager Started");
                WriteLog($"Started: {DateTime.Now}");
                WriteLog("========================================");

                ShowMenu();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("========================================");
                Console.WriteLine("КРИТИЧЕСКАЯ ОШИБКА!");
                Console.WriteLine("========================================");
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.ResetColor();
                WriteLog($"CRITICAL ERROR: {ex.Message}");
                WriteLog($"Stack trace: {ex.StackTrace}");
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }

        static void ShowMenu()
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("========================================");
                    Console.WriteLine("    DNS Manager");
                    Console.WriteLine("    Управление DNS серверами");
                    Console.WriteLine("    Best for: Roblox, Telegram, Discord, YouTube");
                    Console.WriteLine("    Works with: Wired (Ethernet) and Wi-Fi");
                    Console.WriteLine("========================================");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("Выберите действие:");
                    Console.WriteLine("  1. Установить DNS (выбор провайдера)");
                    Console.WriteLine("  2. Вернуть DNS к автоматическим настройкам (DHCP)");
                    Console.WriteLine("  3. Показать текущие настройки DNS");
                    Console.WriteLine("  4. Очистить DNS кеш");
                    Console.WriteLine("  5. Показать лог");
                    Console.WriteLine("  0. Выход");
                    Console.WriteLine();
                    Console.Write("Введите номер: ");

                    string? choice = Console.ReadLine();

                    switch (choice ?? "")
                    {
                        case "1":
                            DNSProvider? selectedProvider = ChooseDNSProvider();
                            if (selectedProvider != null)
                            {
                                SetCustomDNS(selectedProvider);
                            }
                            break;
                        case "2":
                            RestoreAutoDNS();
                            break;
                        case "3":
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("========================================");
                            Console.WriteLine("    Текущие настройки DNS");
                            Console.WriteLine("========================================");
                            Console.ResetColor();
                            ShowCurrentDNS();
                            Console.WriteLine();
                            Console.WriteLine("Нажмите любую клавишу для продолжения...");
                            Console.ReadKey();
                            break;
                        case "4":
                            FlushDNSCache();
                            break;
                        case "5":
                            ShowLog();
                            break;
                        case "0":
                            WriteLog("Program exited by user");
                            Console.WriteLine();
                            Console.WriteLine("До свидания!");
                            System.Threading.Thread.Sleep(500);
                            return;
                        default:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Неверный выбор! Попробуйте снова.");
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(1500);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine($"Ошибка в меню: {ex.Message}");
                    Console.ResetColor();
                    WriteLog($"ERROR in menu: {ex.Message}");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static DNSProvider? ChooseDNSProvider()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("========================================");
                Console.WriteLine("    Выбор DNS провайдера");
                Console.WriteLine("========================================");
                Console.ResetColor();
                Console.WriteLine();

                for (int i = 0; i < dnsProviders.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"  {i + 1}. {dnsProviders[i].Name}");
                    Console.ResetColor();
                    Console.WriteLine($" ({dnsProviders[i].PrimaryDNS}, {dnsProviders[i].SecondaryDNS})");
                    Console.WriteLine($"     {dnsProviders[i].Description}");
                    Console.WriteLine();
                }

                Console.WriteLine("  0. Вернуться в главное меню");
                Console.WriteLine();
                Console.Write("Введите номер DNS провайдера (1-{0}) или 0 для выхода: ", dnsProviders.Count);
                string? choice = Console.ReadLine();

                if (choice == "0")
                {
                    return null; // Возврат в главное меню
                }

                if (int.TryParse(choice, out int index) && index >= 1 && index <= dnsProviders.Count)
                {
                    return dnsProviders[index - 1];
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Неверный выбор! Попробуйте снова.");
                    Console.ResetColor();
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void SetCustomDNS(DNSProvider provider)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================");
            Console.WriteLine($"    Установка {provider.Name} DNS");
            Console.WriteLine("========================================");
            Console.ResetColor();
            Console.WriteLine();

            WriteLog($"Starting {provider.Name} DNS setup...");
            WriteLog($"DNS Servers: {provider.PrimaryDNS}, {provider.SecondaryDNS}");

            List<NetworkInterface> adapters = GetActiveNetworkAdapters();
            
            if (adapters.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка: Не найдены активные сетевые адаптеры!");
                Console.ResetColor();
                WriteLog("ERROR: No active network adapters found");
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Найдено активных адаптеров: {adapters.Count}");
            Console.WriteLine();
            WriteLog($"Found {adapters.Count} active adapters");

            int successCount = 0;

            foreach (var adapter in adapters)
            {
                Console.WriteLine($"[{successCount + 1}] Настройка DNS для: {adapter.Name}");
                WriteLog($"Setting DNS for: {adapter.Name}");

                bool success = SetDNSForAdapter(adapter.Name, provider.PrimaryDNS, provider.SecondaryDNS);
                
                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("      [OK] DNS установлен успешно");
                    Console.ResetColor();
                    WriteLog($"SUCCESS: DNS set for {adapter.Name}");
                    successCount++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("      [ОШИБКА] Не удалось установить DNS");
                    Console.ResetColor();
                    WriteLog($"ERROR: Failed to set DNS for {adapter.Name}");
                }
                Console.WriteLine();
            }

            if (successCount > 0)
            {
                Console.WriteLine("Применение изменений...");
                FlushDNSCache();
                RefreshNetworkSettings();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[УСПЕХ] DNS установлен для {successCount} подключений!");
                Console.WriteLine($"Используется: {provider.Name} ({provider.PrimaryDNS}, {provider.SecondaryDNS})");
                Console.ResetColor();
                WriteLog($"SUCCESS: DNS set for {successCount} connection(s)");

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("✓ Изменения применены сразу, перезагрузка ПК НЕ требуется!");
                Console.WriteLine("  Просто перезапустите браузер и приложения (Discord, Telegram и т.д.)");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Текущие настройки DNS:");
                ShowCurrentDNS();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ОШИБКА] Не удалось установить DNS автоматически.");
                Console.ResetColor();
                WriteLog("ERROR: Failed to set DNS automatically");
                Console.WriteLine();
                Console.WriteLine("Попробуйте установить DNS вручную:");
                Console.WriteLine("1. Нажмите Win+R, введите: ncpa.cpl");
                Console.WriteLine("2. Правой кнопкой на ваше подключение -> Свойства");
                Console.WriteLine("3. Протокол IPv4 -> Свойства");
                Console.WriteLine($"4. Используйте DNS: {provider.PrimaryDNS} (предпочитаемый) и {provider.SecondaryDNS} (альтернативный)");
            }

            WriteLog($"{provider.Name} DNS setup finished");
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void RestoreAutoDNS()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================");
            Console.WriteLine("    Возврат DNS к автоматическим настройкам");
            Console.WriteLine("    (DNS от провайдера)");
            Console.WriteLine("========================================");
            Console.ResetColor();
            Console.WriteLine();

            WriteLog("Starting DNS restore to DHCP...");

            List<NetworkInterface> adapters = GetActiveNetworkAdapters();

            if (adapters.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка: Не найдены активные сетевые адаптеры!");
                Console.ResetColor();
                WriteLog("ERROR: No active network adapters found");
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Найдено активных адаптеров: {adapters.Count}");
            Console.WriteLine();
            WriteLog($"Found {adapters.Count} active adapters");

            int successCount = 0;

            foreach (var adapter in adapters)
            {
                Console.WriteLine($"[{successCount + 1}] Возврат DNS для: {adapter.Name}");
                WriteLog($"Restoring DNS for: {adapter.Name}");

                bool success = RestoreDNSForAdapter(adapter.Name);

                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("      [OK] DNS возвращен к автоматическим настройкам");
                    Console.ResetColor();
                    WriteLog($"SUCCESS: DNS restored for {adapter.Name}");
                    successCount++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("      [Пропущено] Не IPv4 подключение или ошибка");
                    Console.ResetColor();
                    WriteLog($"WARNING: Failed to restore DNS for {adapter.Name}");
                }
                Console.WriteLine();
            }

            if (successCount > 0)
            {
                Console.WriteLine("Применение изменений...");
                FlushDNSCache();
                RefreshNetworkSettings();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[УСПЕХ] DNS возвращен к автоматическим настройкам для {successCount} подключений!");
                Console.ResetColor();
                WriteLog($"SUCCESS: DNS restored for {successCount} connection(s)");

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("✓ Изменения применены сразу, перезагрузка ПК НЕ требуется!");
                Console.WriteLine("  Просто перезапустите браузер и приложения.");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Текущие настройки DNS:");
                ShowCurrentDNS();
                Console.WriteLine();
                Console.WriteLine("Готово! DNS теперь получается автоматически от провайдера.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ОШИБКА] Не удалось вернуть DNS автоматически.");
                Console.ResetColor();
                WriteLog("ERROR: Failed to restore DNS automatically");
                Console.WriteLine();
                Console.WriteLine("Попробуйте вернуть DNS вручную:");
                Console.WriteLine("1. Нажмите Win+R, введите: ncpa.cpl");
                Console.WriteLine("2. Правой кнопкой на ваше подключение -> Свойства");
                Console.WriteLine("3. Протокол IPv4 -> Свойства");
                Console.WriteLine("4. Выберите 'Получить адрес DNS-сервера автоматически'");
            }

            WriteLog("DNS restore finished");
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static DNSProvider? DetectDNSProvider(string dnsAddress)
        {
            foreach (var provider in dnsProviders)
            {
                if (dnsAddress == provider.PrimaryDNS || dnsAddress == provider.SecondaryDNS)
                {
                    return provider;
                }
            }
            return null;
        }

        static void ShowCurrentDNS()
        {
            Console.WriteLine();
            WriteLog("Showing current DNS settings");

            bool foundAny = false;
            List<string> foundDNSAddresses = new List<string>();

            try
            {
                // Метод 1: Через ipconfig (более подробная информация)
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "ipconfig",
                    Arguments = "/all",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process? process = Process.Start(psi))
                {
                    if (process != null && process.StandardOutput != null)
                    {
                        // Читаем вывод до завершения процесса
                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("DNS серверы (из ipconfig):");
                        Console.ResetColor();

                        foreach (string line in lines)
                        {
                            string trimmed = line.Trim();
                            if (trimmed.Contains("DNS Servers") && !trimmed.Contains("::") && !trimmed.Contains("IPv6"))
                            {
                                // Извлекаем IP адрес из строки
                                string[] parts = trimmed.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string part in parts)
                                {
                                    if (System.Net.IPAddress.TryParse(part, out _))
                                    {
                                        if (!foundDNSAddresses.Contains(part))
                                        {
                                            foundDNSAddresses.Add(part);
                                        }
                                    }
                                }
                                Console.WriteLine("  " + trimmed);
                                foundAny = true;
                            }
                        }
                    }
                }

                // Метод 2: Через NetworkInterface (альтернативный способ)
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Активные сетевые адаптеры:");
                Console.ResetColor();

                List<NetworkInterface> adapters = GetActiveNetworkAdapters();
                if (adapters.Count > 0)
                {
                    foreach (var adapter in adapters)
                    {
                        Console.WriteLine($"  {adapter.Name} ({adapter.NetworkInterfaceType})");
                        Console.WriteLine($"    Статус: {adapter.OperationalStatus}");
                        
                        // Получаем IP свойства
                        try
                        {
                            var ipProps = adapter.GetIPProperties();
                            if (ipProps.DnsAddresses.Count > 0)
                            {
                                Console.WriteLine("    DNS серверы:");
                                foreach (var dns in ipProps.DnsAddresses)
                                {
                                    string dnsStr = dns.ToString();
                                    Console.WriteLine($"      - {dnsStr}");
                                    
                                    // Добавляем в список для определения провайдера
                                    if (!foundDNSAddresses.Contains(dnsStr))
                                    {
                                        foundDNSAddresses.Add(dnsStr);
                                    }
                                    foundAny = true;
                                }
                            }
                            else
                            {
                                Console.WriteLine("    DNS: Автоматически (DHCP)");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("    DNS: Не удалось получить информацию");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("  Активные адаптеры не найдены");
                }

                // Определение установленного DNS провайдера
                if (foundDNSAddresses.Count > 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Определенный DNS провайдер:");
                    Console.ResetColor();
                    
                    bool providerFound = false;
                    foreach (string dnsAddr in foundDNSAddresses)
                    {
                        DNSProvider? provider = DetectDNSProvider(dnsAddr);
                        if (provider != null)
                        {
                            Console.WriteLine($"  ✓ {provider.Name} ({provider.PrimaryDNS}, {provider.SecondaryDNS})");
                            Console.WriteLine($"    {provider.Description}");
                            providerFound = true;
                            break; // Берем первый найденный провайдер
                        }
                    }
                    
                    if (!providerFound)
                    {
                        Console.WriteLine("  ⚠ Неизвестный DNS провайдер или пользовательский DNS");
                        Console.WriteLine($"  Текущие DNS: {string.Join(", ", foundDNSAddresses)}");
                    }
                }
                else if (!foundAny)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("DNS настройки не найдены или получаются автоматически от провайдера (DHCP).");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка при получении настроек DNS: {ex.Message}");
                Console.ResetColor();
                WriteLog($"ERROR getting DNS settings: {ex.Message}");
            }
        }

        static void FlushDNSCache()
        {
            WriteLog("Flushing DNS cache");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "ipconfig",
                Arguments = "/flushdns",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using (Process? process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        if (process.StandardOutput != null)
                        {
                            string output = process.StandardOutput.ReadToEnd();
                        }
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("  [OK] DNS кеш очищен");
                            Console.ResetColor();
                            WriteLog("SUCCESS: DNS cache flushed");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("  [Предупреждение] Не удалось очистить DNS кеш");
                            Console.ResetColor();
                            WriteLog("WARNING: Failed to flush DNS cache");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  Ошибка при очистке DNS кеша: {ex.Message}");
                Console.ResetColor();
                WriteLog($"ERROR flushing DNS cache: {ex.Message}");
            }
        }

        static void RefreshNetworkSettings()
        {
            WriteLog("Refreshing network settings");

            try
            {
                // Обновление сетевых настроек через netsh
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "interface ip set dns",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                // Обновление через ipconfig /renew (опционально, может занять время)
                // Пропускаем, так как это может прервать соединение

                // Перезапуск службы DNS Client (требует прав администратора)
                try
                {
                    ProcessStartInfo dnsService = new ProcessStartInfo
                    {
                        FileName = "net",
                        Arguments = "stop dnscache",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    using (Process? stopProcess = Process.Start(dnsService))
                    {
                        if (stopProcess != null)
                        {
                            stopProcess.WaitForExit();
                            System.Threading.Thread.Sleep(500);

                            // Запускаем службу обратно
                            dnsService.Arguments = "start dnscache";
                            using (Process? startProcess = Process.Start(dnsService))
                            {
                                if (startProcess != null)
                                {
                                    startProcess.WaitForExit();
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("  [OK] Служба DNS обновлена");
                                    Console.ResetColor();
                                    WriteLog("SUCCESS: DNS service refreshed");
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Игнорируем ошибки перезапуска службы - не критично
                    WriteLog("INFO: DNS service restart skipped (not critical)");
                }
            }
            catch (Exception ex)
            {
                WriteLog($"WARNING refreshing network: {ex.Message}");
            }
        }

        static void ShowLog()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================");
            Console.WriteLine("    Лог файл");
            Console.WriteLine("========================================");
            Console.ResetColor();
            Console.WriteLine();

            if (File.Exists(logFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(logFile);
                    foreach (string line in lines)
                    {
                        Console.WriteLine(line);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ошибка при чтении лог файла: {ex.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.WriteLine("Лог файл не найден.");
            }

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static List<NetworkInterface> GetActiveNetworkAdapters()
        {
            List<NetworkInterface> adapters = new List<NetworkInterface>();

            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface ni in interfaces)
                {
                    if (ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                         ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        adapters.Add(ni);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"ERROR getting network adapters: {ex.Message}");
            }

            return adapters;
        }

        static bool SetDNSForAdapter(string adapterName, string primaryDNS, string secondaryDNS)
        {
            try
            {
                // Установка основного DNS
                ProcessStartInfo psi1 = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"interface ipv4 set dns name=\"{adapterName}\" static {primaryDNS}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process? process1 = Process.Start(psi1))
                {
                    if (process1 != null)
                    {
                        process1.WaitForExit();
                        if (process1.ExitCode != 0)
                        {
                            WriteLog($"ERROR: Failed to set primary DNS for {adapterName}");
                            return false;
                        }
                    }
                    else
                    {
                        WriteLog($"ERROR: Failed to start process for {adapterName}");
                        return false;
                    }
                }

                // Установка вторичного DNS
                ProcessStartInfo psi2 = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"interface ipv4 add dns name=\"{adapterName}\" {secondaryDNS} index=2",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process? process2 = Process.Start(psi2))
                {
                    if (process2 != null)
                    {
                        process2.WaitForExit();
                        if (process2.ExitCode != 0)
                        {
                            WriteLog($"WARNING: Primary DNS set for {adapterName}, but secondary failed");
                            return true; // Основной DNS установлен, это успех
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                WriteLog($"ERROR setting DNS for {adapterName}: {ex.Message}");
                return false;
            }
        }

        static bool RestoreDNSForAdapter(string adapterName)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"interface ipv4 set dns name=\"{adapterName}\" dhcp",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process? process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        return process.ExitCode == 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLog($"ERROR restoring DNS for {adapterName}: {ex.Message}");
                return false;
            }
        }

        static bool IsRunAsAdministrator()
        {
            try
            {
                System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        static void WriteLog(string message)
        {
            try
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                File.AppendAllText(logFile, logMessage + Environment.NewLine);
            }
            catch
            {
                // Игнорируем ошибки записи в лог
            }
        }
    }
}

