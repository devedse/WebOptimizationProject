using DeveCoolLib.DeveConsoleMenu;
using DeveCoolLib.Logging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOptimizationProject.Configuration;
using WebOptimizationProject.Helpers.Git;

namespace WebOptimizationProject
{
    public class WopRepoManager
    {
        private readonly WopConfig _wopConfig;
        private readonly GitOctoKitHandler _gitOctoKitHandler;

        public WopRepoManager(WopConfig wopConfig)
        {
            _wopConfig = wopConfig;

            _gitOctoKitHandler = new GitOctoKitHandler(_wopConfig);
        }

        public void Start()
        {
            var consoleMenu = new ConsoleMenu(ConsoleMenuType.StringInput);

            consoleMenu.MenuOptions.Add(new ConsoleMenuOption("Remove all my forks and close their active PR's", new Action(() =>
            {
                Console.WriteLine("Actions to execute:");

                var allPrs = _gitOctoKitHandler.GetAllMyOpenPrs().Result;

                foreach (var pr in allPrs)
                {
                    Console.WriteLine($"\tClose:\t{pr.Issue.HtmlUrl} ({pr.Issue.State})");
                }

                var myReposAll = _gitOctoKitHandler.GitHubClient.Repository.GetAllForCurrent().Result;
                var myRepos = myReposAll.Where(t => t.Fork).ToList();

                foreach (var repo in myRepos)
                {
                    Console.WriteLine($"\tDelete:\t{repo.FullName}");
                }

                var consMenuRemoveRepos = new ConsoleMenu(ConsoleMenuType.StringInput);

                consMenuRemoveRepos.MenuOptions.Add(new ConsoleMenuOption("Yes", new Action(() =>
                {
                    foreach (var pr in allPrs)
                    {
                        Console.Write($"Closing: {pr.Issue.HtmlUrl} ");
                        var up = new PullRequestUpdate()
                        {
                            State = ItemState.Closed
                        };
                        _gitOctoKitHandler.GitHubClient.PullRequest.Update(pr.RepoOwner, pr.RepoName, pr.Issue.Number, up).Wait();
                        Console.WriteLine("Done");
                    }

                    foreach (var repo in myRepos)
                    {
                        Console.Write($"Removing: {repo.FullName} ");
                        _gitOctoKitHandler.GitHubClient.Repository.Delete(repo.Id).Wait();
                        Console.WriteLine("Done");
                    }
                })));

                consMenuRemoveRepos.MenuOptions.Add(new ConsoleMenuOption("No", new Action(() =>
                {

                })));

                Console.WriteLine();
                Console.WriteLine("Are you sure you want to execute these actions?");
                consMenuRemoveRepos.RenderMenu();
                consMenuRemoveRepos.WaitForResult();
            })));

            consoleMenu.MenuOptions.Add(new ConsoleMenuOption("Remove all my forks", new Action(() =>
            {
                Console.WriteLine("Actions to execute:");

                var myReposAll = _gitOctoKitHandler.GitHubClient.Repository.GetAllForCurrent().Result;
                var myRepos = myReposAll.Where(t => t.Fork).ToList();

                foreach (var repo in myRepos)
                {
                    Console.WriteLine($"\tDelete:\t{repo.FullName}");
                }

                var consMenuRemoveRepos = new ConsoleMenu(ConsoleMenuType.StringInput);

                consMenuRemoveRepos.MenuOptions.Add(new ConsoleMenuOption("Yes", new Action(() =>
                {
                    foreach (var repo in myRepos)
                    {
                        Console.Write($"Removing: {repo.FullName} ");
                        _gitOctoKitHandler.GitHubClient.Repository.Delete(repo.Id).Wait();
                        Console.WriteLine("Done");
                    }
                })));

                consMenuRemoveRepos.MenuOptions.Add(new ConsoleMenuOption("No", new Action(() =>
                {

                })));

                Console.WriteLine();
                Console.WriteLine("Are you sure you want to execute these actions?");
                consMenuRemoveRepos.RenderMenu();
                consMenuRemoveRepos.WaitForResult();
            })));

            consoleMenu.MenuOptions.Add(new ConsoleMenuOption("Close all my pull requests", new Action(() =>
            {
                Console.WriteLine("Actions to execute:");

                var allPrs = _gitOctoKitHandler.GetAllMyOpenPrs().Result;

                foreach (var pr in allPrs)
                {
                    Console.WriteLine($"\tClose:\t{pr.Issue.HtmlUrl} ({pr.Issue.State})");
                }

                var consMenuRemoveRepos = new ConsoleMenu(ConsoleMenuType.StringInput);

                consMenuRemoveRepos.MenuOptions.Add(new ConsoleMenuOption("Yes", new Action(() =>
                {
                    foreach (var pr in allPrs)
                    {
                        Console.Write($"Closing: {pr.Issue.HtmlUrl} ");
                        var up = new PullRequestUpdate()
                        {
                            State = ItemState.Closed
                        };
                        _gitOctoKitHandler.GitHubClient.PullRequest.Update(pr.RepoOwner, pr.RepoName, pr.Issue.Number, up).Wait();
                        Console.WriteLine("Done");
                    }
                })));

                consMenuRemoveRepos.MenuOptions.Add(new ConsoleMenuOption("No", new Action(() =>
                {

                })));

                Console.WriteLine();
                Console.WriteLine("Are you sure you want to execute these actions?");
                consMenuRemoveRepos.RenderMenu();
                consMenuRemoveRepos.WaitForResult();
            })));

            consoleMenu.MenuOptions.Add(new ConsoleMenuOption("Exit", new Action(() =>
            {

            })));

            Console.WriteLine();
            consoleMenu.RenderMenu();
            consoleMenu.WaitForResult();
        }
    }
}
