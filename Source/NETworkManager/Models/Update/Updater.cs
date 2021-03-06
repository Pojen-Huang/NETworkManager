﻿using System;
using System.Threading.Tasks;
using Octokit;
using NETworkManager.Models.Settings;
using NETworkManager.Properties;

namespace NETworkManager.Models.Update
{
    public class Updater
    {
        #region Events
        public event EventHandler<UpdateAvailableArgs> UpdateAvailable;

        protected virtual void OnUpdateAvailable(UpdateAvailableArgs e)
        {
            UpdateAvailable?.Invoke(this, e);
        }

        public event EventHandler NoUpdateAvailable;

        protected virtual void OnNoUpdateAvailable()
        {
            NoUpdateAvailable?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Error;

        protected virtual void OnError()
        {
            Error?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Methods
        public void Check()
        {
            Task.Run(() =>
            {
                try
                {
                    GitHubClient client = new GitHubClient(new ProductHeaderValue(Resources.NETworkManager_ProjectName));

                    Task<Release> latestRelease = client.Repository.Release.GetLatest(Resources.NETworkManager_GitHub_User, Resources.NETworkManager_GitHub_Repo);

                    Version latestVersion = new Version(latestRelease.Result.TagName.TrimStart('v'));

                    // Compare versions (tag=v1.4.2.0, version=1.4.2.0)
                    if (latestVersion > AssemblyManager.Current.Version)
                        OnUpdateAvailable(new UpdateAvailableArgs(latestVersion));
                    else
                        OnNoUpdateAvailable();
                }
                catch
                {
                    OnError();
                }
            });
        }
        #endregion
    }
}
