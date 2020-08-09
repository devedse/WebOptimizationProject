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
                var myUser = _gitOctoKitHandler.GitHubClient.User.Current().Result;
                var myReposAll = _gitOctoKitHandler.GitHubClient.Repository.GetAllForCurrent().Result;
                var myReposTemp = myReposAll.Where(t => t.Fork).ToList();

                Console.WriteLine("Actions to execute:");

                var prDict = new Dictionary<Repository, List<PullRequest>>();

                foreach (var repo in myReposTemp)
                {
                    var prsHereList = new List<PullRequest>();
                    var thisRepo = _gitOctoKitHandler.GitHubClient.Repository.Get(repo.Id).Result;
                    prDict.Add(thisRepo, prsHereList);

                    var prsHere = _gitOctoKitHandler.GitHubClient.PullRequest.GetAllForRepository(thisRepo.Parent.Id).Result;
                    var prsFiltered = prsHere.Where(t => t.State.Value == ItemState.Open && t.User.Id == myUser.Id).ToList();
                    foreach (var pr in prsHere)
                    {
                        Console.WriteLine($"\tClose:\t{pr.HtmlUrl} ({pr.State})");
                        prsHereList.Add(pr);
                    }

                    Console.WriteLine($"\tDelete:\t{repo.FullName}");
                }

                var consMenuRemoveRepos = new ConsoleMenu(ConsoleMenuType.StringInput);

                consMenuRemoveRepos.MenuOptions.Add(new ConsoleMenuOption("Yes", new Action(() =>
                {
                    foreach (var kvp in prDict)
                    {
                        var repo = kvp.Key;
                        foreach (var pr in kvp.Value)
                        {
                            Console.Write($"Closing: {pr.HtmlUrl} ");
                            var up = new PullRequestUpdate()
                            {
                                State = ItemState.Closed
                            };
                            _gitOctoKitHandler.GitHubClient.PullRequest.Update(repo.Parent.Id, pr.Number, up).Wait();
                            Console.WriteLine("Done");
                        }

                        Console.Write($"Removing: {repo.FullName} ");
                        _gitOctoKitHandler.GitHubClient.Repository.Delete(repo.Id).Wait();
                        Console.WriteLine("Done");
                    }
                })));

                consMenuRemoveRepos.MenuOptions.Add(new ConsoleMenuOption("No", new Action(() =>
                {

                })));

                Console.WriteLine();
                Console.WriteLine("Are you sure you want to remove all these repositories?");
                consMenuRemoveRepos.RenderMenu();
                consMenuRemoveRepos.WaitForResult();
            })));

            consoleMenu.MenuOptions.Add(new ConsoleMenuOption("Remove all my forks", new Action(() =>
            {
                var myReposAll = _gitOctoKitHandler.GitHubClient.Repository.GetAllForCurrent().Result;
                var myRepos = myReposAll.Where(t => t.Fork).ToList();

                Console.WriteLine("All these repositories will be removed:");

                foreach (var repo in myRepos)
                {
                    Console.WriteLine($"\t{repo.FullName}");
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
                Console.WriteLine("Are you sure you want to remove all these repositories?");
                consMenuRemoveRepos.RenderMenu();
                consMenuRemoveRepos.WaitForResult();
            })));

            consoleMenu.MenuOptions.Add(new ConsoleMenuOption("Close all my pull requests", new Action(() =>
            {
                var myIssues = _gitOctoKitHandler.GitHubClient.Issue.GetAllForCurrent(new IssueRequest() { Filter = IssueFilter.All, State = ItemStateFilter.All }, new ApiOptions() { }).Result;

                Console.WriteLine("All these issues will be closed:");

                foreach (var issue in myIssues)
                {
                    Console.WriteLine($"\t{issue.HtmlUrl} ({issue.State})");
                }

                Console.WriteLine("Yeah this doesn't work yet because for some reason you can't obtain all your pull requests. I made a support request for this");

                var consMenuRemoveRepos = new ConsoleMenu(ConsoleMenuType.StringInput);

                consMenuRemoveRepos.MenuOptions.Add(new ConsoleMenuOption("Yes", new Action(() =>
                {
                    Console.WriteLine("Yeahhh. this is sadly not yet implemented");
                })));

                consMenuRemoveRepos.MenuOptions.Add(new ConsoleMenuOption("No", new Action(() =>
                {

                })));

                Console.WriteLine();
                Console.WriteLine("Are you sure you want to remove all these repositories?");
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
