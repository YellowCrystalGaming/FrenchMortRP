using Life;
using Life.DB;
using Life.Network;
using UnityEngine;
using MyMenu;
using UIPanelManager;
using MyMenu.Entities;
using System;
using Life.UI;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Config.config;
using Newtonsoft.Json;

namespace FrenchMortRP
{
    public class FrenchMortRP : Plugin
    {
        Dictionary<uint, long> Temps = new Dictionary<uint, long>();
        private string _webhook;
        private string _roleplayKillTexte;
        private string _caractereKillTexte;
        private int _temps;

        public FrenchMortRP(IGameAPI api) : base(api)
        {
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            Debug.Log("FrenchMortRP  a été initialisé avec succès!");

            Section section = new Section(Section.GetSourceName(), Section.GetSourceName(), "v1.0.0", "French Aero");
            Action<UIPanel> action = ui => MaFonction(section.GetPlayer(ui));
            section.OnlyAdmin = true;
            section.MinAdminLevel = 1;
            section.Line = new UITabLine(section.Title, action);
            section.Insert(true);

            var configFilePath = Path.Combine(pluginsPath, "FrenchMortRP/config.json");
            var globalConfiguration = ChargerConfiguration(configFilePath);
            _webhook = globalConfiguration.webhook;
            _roleplayKillTexte = globalConfiguration.roleplayKillTexte;
            _caractereKillTexte = globalConfiguration.caractereKillTexte;
            _temps = globalConfiguration.temps;
        }

        public void MaFonction(Player player)
        {
            UIPanel panel = new UIPanel("MortRP", UIPanel.PanelType.Tab);
            panel.SetTitle("Mort RP");
            panel.AddTabLine("Character Kill", (Action<UIPanel>)(ui =>
            {
                Player player1 = player.GetClosestPlayer(true);
                if (player1 != null)
                {
                    player.ClosePanel(panel);
                    DateTime Temps2 = DateTime.Now;
                    if (Temps.ContainsKey(player.netId))
                    {
                        if (Temps2.Ticks - Temps[player.netId] > TimeSpan.FromMinutes(_temps).Ticks)
                        {
                            UIPanel panel4 = new UIPanel("MortRP", UIPanel.PanelType.Text);
                            panel4.SetTitle("Mort RP");
                            panel4.SetText("Êtes vous sûre de votre action?");
                            panel4.AddButton("Oui", (Action<UIPanel>)(ui4 =>
                            {
                                Temps[player.netId] = Temps2.Ticks;
                                player.ClosePanel(panel4);
                                UIPanel panel5 = new UIPanel("MortRP", UIPanel.PanelType.Text);
                                panel5.SetTitle("Mort RP");
                                panel5.SetText(_caractereKillTexte);
                                panel5.AddButton("Oui", (Action<UIPanel>)(ui5 =>
                                {
                                    player1.ClosePanel(panel5);
                                    UIPanel panel6 = new UIPanel("MortRP", UIPanel.PanelType.Input);
                                    panel6.SetTitle("Mort RP");
                                    panel6.SetInputPlaceholder("Entrez votre prénom.");
                                    panel6.AddButton("Valider", (Action<UIPanel>)(ui6 =>
                                    {
                                        if (panel6.inputText != null)
                                        {
                                            if (panel6.inputText != "")
                                            {
                                                player1.character.Firstname = panel6.inputText;
                                                player1.ClosePanel(panel6);
                                                UIPanel panel7 = new UIPanel("MortRP", UIPanel.PanelType.Input);
                                                panel7.SetTitle("Mort RP");
                                                panel7.SetInputPlaceholder("Entrez votre nom.");
                                                panel7.AddButton("Valider", (Action<UIPanel>)(ui7 =>
                                                {
                                                    if (panel7.inputText != null)
                                                    {
                                                        if (panel7.inputText != "")
                                                        {
                                                            player1.character.Lastname = panel7.inputText;
                                                            PanelManager.Notification(player1, "Mort RP", "Vous vous appeller désormais " + panel6.inputText + " " + panel7.inputText + ".", NotificationManager.Type.Success);
                                                            player1.ClosePanel(panel7);
                                                            SendWebhook(_webhook, "Le staff " + player.GetFullName() + " a envoyer une possibilité de mort rp caractère.");
                                                        }
                                                        else
                                                        {
                                                            PanelManager.Notification(player, "PV", "Vous devez mettre un nom!", NotificationManager.Type.Warning);
                                                        };
                                                    }
                                                    else
                                                    {
                                                        PanelManager.Notification(player, "PV", "Vous devez mettre un nom!", NotificationManager.Type.Warning);
                                                    };
                                                }));
                                            }
                                            else
                                            {
                                                PanelManager.Notification(player, "PV", "Vous devez mettre un prénom!", NotificationManager.Type.Warning);
                                            };
                                        }
                                        else
                                        {
                                            PanelManager.Notification(player, "PV", "Vous devez mettre un prénom!", NotificationManager.Type.Warning);
                                        };
                                    }));
                                    player1.ShowPanelUI(panel6);
                                }));
                                panel5.AddButton("Non", (Action<UIPanel>)(ui6 =>
                                {
                                    player1.ClosePanel(panel5);
                                }));
                                player1.ShowPanelUI(panel5);
                            }));
                            panel4.AddButton("Non", (Action<UIPanel>)(ui4 =>
                            {
                                player.ClosePanel(panel4);
                            }));
                            player.ShowPanelUI(panel4);
                        }
                        else
                        {
                            PanelManager.Notification(player, "Character Kill", "Vous ne pouvez pas mettre de PV plus de toutes les " + _temps + " minutes", NotificationManager.Type.Warning);
                        }
                    }
                    else
                    {
                        UIPanel panel4 = new UIPanel("MortRP", UIPanel.PanelType.Text);
                        panel4.SetTitle("Mort RP");
                        panel4.SetText("Êtes vous sûre de votre action?");
                        panel4.AddButton("Oui", (Action<UIPanel>)(ui4 =>
                        {
                            Temps.Add(player.netId, Temps2.Ticks);
                            player.ClosePanel(panel4);
                            UIPanel panel5 = new UIPanel("MortRP", UIPanel.PanelType.Text);
                            panel5.SetTitle("Mort RP");
                            panel5.SetText(_caractereKillTexte);
                            panel5.AddButton("Oui", (Action<UIPanel>)(ui5 =>
                            {
                                player1.ClosePanel(panel5);
                                UIPanel panel6 = new UIPanel("MortRP", UIPanel.PanelType.Input);
                                panel6.SetTitle("Mort RP");
                                panel6.SetInputPlaceholder("Entrez votre prénom.");
                                panel6.AddButton("Valider", (Action<UIPanel>)(ui6 =>
                                {
                                    if (panel6.inputText != null)
                                    {
                                        if (panel6.inputText != "")
                                        {
                                            player1.character.Firstname = panel6.inputText;
                                            player1.ClosePanel(panel6);
                                            UIPanel panel7 = new UIPanel("MortRP", UIPanel.PanelType.Input);
                                            panel7.SetTitle("Mort RP");
                                            panel7.SetInputPlaceholder("Entrez votre nom.");
                                            panel7.AddButton("Valider", (Action<UIPanel>)(ui7 =>
                                            {
                                                if (panel7.inputText != null)
                                                {
                                                    if (panel7.inputText != "")
                                                    {
                                                        player1.character.Lastname = panel7.inputText;
                                                        PanelManager.Notification(player1, "Mort RP", "Vous vous appeller désormais " + panel6.inputText + " " + panel7.inputText + ".", NotificationManager.Type.Success);
                                                        player1.ClosePanel(panel7);
                                                        SendWebhook(_webhook, "Le staff " + player.GetFullName() + " a envoyer une possibilité de mort rp caractère.");
                                                    }
                                                    else
                                                    {
                                                        PanelManager.Notification(player, "PV", "Vous devez mettre un nom!", NotificationManager.Type.Warning);
                                                    };
                                                }
                                                else
                                                {
                                                    PanelManager.Notification(player, "PV", "Vous devez mettre un nom!", NotificationManager.Type.Warning);
                                                };
                                            }));
                                            player1.ShowPanelUI(panel7);
                                        }
                                        else
                                        {
                                            PanelManager.Notification(player, "PV", "Vous devez mettre un prénom!", NotificationManager.Type.Warning);
                                        };
                                    }
                                    else
                                    {
                                        PanelManager.Notification(player, "PV", "Vous devez mettre un prénom!", NotificationManager.Type.Warning);
                                    };
                                }));
                                player1.ShowPanelUI(panel6);
                            }));
                            panel5.AddButton("Non", (Action<UIPanel>)(ui6 =>
                            {
                                player1.ClosePanel(panel5);
                            }));
                            player1.ShowPanelUI(panel5);
                        }));
                        panel4.AddButton("Non", (Action<UIPanel>)(ui4 =>
                        {
                            player.ClosePanel(panel4);
                        }));
                        player.ShowPanelUI(panel4);
                    };
                }
                else
                    PanelManager.Notification(player, "Roleplay Kill", "Vous n'êtes pas à côté d'un joueur!", NotificationManager.Type.Warning);

            }));
            panel.AddTabLine("Roleplay Kill", (Action<UIPanel>)(ui =>
            {
                Player player1 = player.GetClosestPlayer(true);
                if (player1 != null)
                {
                    player.ClosePanel(panel);
                    DateTime Temps2 = DateTime.Now;
                    if (Temps.ContainsKey(player.netId))
                    {
                        if (Temps2.Ticks - Temps[player.netId] > TimeSpan.FromMinutes(_temps).Ticks)
                        {
                            UIPanel panel2 = new UIPanel("MortRP", UIPanel.PanelType.Text);
                            panel2.SetTitle("Mort RP");
                            panel2.SetText("Êtes vous sûre de votre action?");
                            panel2.AddButton("Oui", (Action<UIPanel>)(ui2 =>
                            {
                                Temps[player.netId] = Temps2.Ticks;
                                player.ClosePanel(panel2);
                                UIPanel panel3 = new UIPanel("MortRP", UIPanel.PanelType.Text);
                                panel3.SetTitle("Mort RP");
                                panel3.SetText(_roleplayKillTexte);
                                panel3.AddButton("Oui", (Action<UIPanel>)(ui3 =>
                                {
                                    player1.Disconnect("Vous avez décidé de mourir.");
                                    LifeDB.DeleteCharacter(player1.character.Id);
                                    player1.Save();
                                    SendWebhook(_webhook, "Le staff " + player.GetFullName() + " a envoyer une possibilité de mort rp roleplay.");
                                }));
                                panel3.AddButton("Non", (Action<UIPanel>)(ui3 =>
                                {
                                    player.ClosePanel(panel3);
                                }));
                                player1.ShowPanelUI(panel3);
                            }));
                            panel2.AddButton("Non", (Action<UIPanel>)(ui2 =>
                            {
                                player.ClosePanel(panel2);
                            }));
                            player.ShowPanelUI(panel2);
                        }
                        else
                        {
                            PanelManager.Notification(player, "Roleplay Kill", "Vous ne pouvez pas mettre de PV plus de toutes les " + _temps + " minutes", NotificationManager.Type.Warning);
                        };
                    }
                    else
                    {
                        UIPanel panel2 = new UIPanel("MortRP", UIPanel.PanelType.Text);
                        panel2.SetTitle("Mort RP");
                        panel2.SetText("Êtes vous sûre de votre action?");
                        panel2.AddButton("Oui", (Action<UIPanel>)(ui2 =>
                        {
                            Temps.Add(player.netId, Temps2.Ticks);
                            player.ClosePanel(panel2);
                            UIPanel panel3 = new UIPanel("MortRP", UIPanel.PanelType.Text);
                            panel3.SetTitle("Mort RP");
                            panel3.SetText(_roleplayKillTexte);
                            panel3.AddButton("Oui", (Action<UIPanel>)(ui3 =>
                            {
                                player1.Disconnect("Vous avez décidé de mourir.");
                                LifeDB.DeleteCharacter(player1.character.Id);
                                player1.Save();
                                SendWebhook(_webhook, "Le staff " + player.GetFullName() + " a envoyer une possibilité de mort rp roleplay.");
                            }));
                            panel3.AddButton("Non", (Action<UIPanel>)(ui3 =>
                            {
                                player.ClosePanel(panel3);
                            }));
                            player1.ShowPanelUI(panel3);
                        }));
                        panel2.AddButton("Non", (Action<UIPanel>)(ui2 =>
                        {
                            player.ClosePanel(panel2);
                        }));
                        player.ShowPanelUI(panel2);
                    };
                }
                else
                    PanelManager.Notification(player, "Roleplay Kill", "Vous n'êtes pas à côté d'un joueur!", NotificationManager.Type.Warning);
            }));
            panel.AddButton("Fermer", (Action<UIPanel>)(ui => player.ClosePanel(ui)));
            panel.AddButton("Valider", (Action<UIPanel>)(ui => panel.SelectTab()));
            player.ShowPanelUI(panel);
        }

        private static MainConfig ChargerConfiguration(string configFilePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(configFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
            }
            if (!File.Exists(configFilePath))
            {
                File.WriteAllText(configFilePath, "{\n  \"Webhook\": null,\n  \"roleplayKillTexte\": null,\n  \"caractereKillTexte\": null,\n  \"temps\": 30\n}");
            }
            var jsonConfig = File.ReadAllText(configFilePath);
            return JsonConvert.DeserializeObject<MainConfig>(jsonConfig);
        }

        private static async Task SendWebhook(string webhookUrl, string content)
        {
            using (var client = new HttpClient())
            {
                var payload = new
                {
                    content = content
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(webhookUrl, data);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erreur lors de l'envoi du webhook. Statut : {response.StatusCode}");
                }
            }
        }
    }
}
