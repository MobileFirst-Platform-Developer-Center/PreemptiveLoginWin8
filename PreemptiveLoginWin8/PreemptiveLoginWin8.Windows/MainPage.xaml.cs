/**
* Copyright 2016 IBM Corp.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Worklight;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PreemptiveLoginWin8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage _this;
        PreemptiveLoginChallengeHandler preemptChallengeHandler;
        IWorklightClient _newClient;

        public MainPage()
        {
            this.InitializeComponent();
            _this = this;
            preemptChallengeHandler = new PreemptiveLoginChallengeHandler("UserLogin");
            preemptChallengeHandler.SetShouldSubmitChallenge(true);
            preemptChallengeHandler.SecurityCheck = "UserLogin";
            preemptChallengeHandler.SetSubmitFailure(false);

            _newClient = WorklightClient.CreateInstance();

            _newClient.RegisterChallengeHandler(preemptChallengeHandler);

            showChallenge(null);
        }

        private async void GetBalance_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                StringBuilder uriBuilder = new StringBuilder().Append("/adapters").Append("/ResourceAdapter").Append("/balance");

                Debug.WriteLine(new Uri(uriBuilder.ToString(), UriKind.Relative));

                WorklightResourceRequest rr = _newClient.ResourceRequest(new Uri(uriBuilder.ToString(), UriKind.Relative), "GET", "accessRestricted");

                WorklightResponse resp = await rr.Send();

                System.Diagnostics.Debug.WriteLine(resp.ResponseText);

                AddTextToConsole(resp.ResponseText);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public void AddTextToConsole(String consoleText)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 async () =>
                 {
                     MainPage._this.Console.Text = "Balance: " + consoleText;

                 });
        }

        public void AddUserName(String userName)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    if (userName != "")
                    {
                        _this.UserName.Text = "Hello " + userName;
                    }

                    else
                    {
                        _this.UserName.Text = "";
                    }
                });

        }

        private void ClearConsole(object sender, DoubleTappedRoutedEventArgs e)
        {
            Console.Text = "";
        }

        private void ShowConsole(object sender, TappedRoutedEventArgs e)
        {
            MainPage._this.ConsolePanel.Visibility = Visibility.Visible;
            MainPage._this.ConsoleTab.Foreground = new SolidColorBrush(Colors.DodgerBlue);
        }
        
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text != "" && password.Text != "")
            {
                JObject userJSON = new JObject();
                userJSON.Add("username", username.Text);
                userJSON.Add("password", password.Text);

                preemptChallengeHandler.challengeAnswer = userJSON;
                PreemptiveLoginChallengeHandler.waitForPincode.Set();
                preemptChallengeHandler.login(userJSON);
            }
            else
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 async () =>
                 {
                     _this.HintText.Text = "Username and password are required";

                 });
            }
            
        }

        public async void showChallenge(Object challenge)
        {
            String errorMsg = "";

            JObject challengeJSON = (JObject)challenge;

            if (challengeJSON != null && challengeJSON.GetValue("errorMsg") != null)
            {
                if (challengeJSON.GetValue("errorMsg").Type == JTokenType.Null)
                    errorMsg = "Wrong Credentials.\n";
            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 async () =>
                 {
                     Console.Text = "";
                     _this.HintText.Text = "";
                     _this.LoginGrid.Visibility = Visibility.Visible;

                     if (challengeJSON != null)
                     {
                         if (errorMsg != "")
                         {
                             _this.HintText.Text = errorMsg + "Remaining Attempts: " + challengeJSON.GetValue("remainingAttempts");
                         }
                         else
                         {
                             _this.HintText.Text = challengeJSON.GetValue("errorMsg") + "\n" + "Remaining Attempts: " + challengeJSON.GetValue("remainingAttempts");
                         }
                     }
                     else
                     {
                         _this.username.Text = "";
                         _this.password.Text = "";
                     }

                     _this.GetBalance.IsEnabled = false;
                     _this.Logout.IsEnabled = false;
                 });
        }

        public void hideChallenge()
        {
            
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    Console.Text = "";
                    _this.LoginGrid.Visibility = Visibility.Collapsed;
                    _this.GetBalance.IsEnabled = true;
                    _this.Logout.IsEnabled = true;
                });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    Console.Text = "";
                });
            preemptChallengeHandler.SetSubmitFailure(true);
            preemptChallengeHandler.SetShouldSubmitChallenge(false);
            PreemptiveLoginChallengeHandler.waitForPincode.Set();

        }

        public WorklightChallengeHandler getChallengeHandler()
        {
            return preemptChallengeHandler;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            preemptChallengeHandler.logout(preemptChallengeHandler.SecurityCheck);
        }
    }
}
