using DeveCoolLib.DeveConsoleMenu;
using DeveCoolLib.Logging;
using Octokit;
using System;
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
