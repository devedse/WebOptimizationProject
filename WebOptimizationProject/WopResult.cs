using DeveImageOptimizer.State;
using Octokit;
using System.Collections.Generic;

namespace WebOptimizationProject
{
    public class WopResult
    {
        public PullRequest CreatedPullRequest { get; set; }
        public IEnumerable<OptimizableFile> OptimizedFiles { get; set; }
    }
}
