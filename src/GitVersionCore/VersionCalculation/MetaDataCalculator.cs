﻿namespace GitVersion.VersionCalculation
{
    using System.Linq;
    using LibGit2Sharp;

    public class MetaDataCalculator : IMetaDataCalculator
    {
        public SemanticVersionBuildMetaData Create(Commit baseVersionSource, GitVersionContext context)
        {
            var qf = new CommitFilter
            {
                IncludeReachableFrom = context.CurrentCommit,
                ExcludeReachableFrom = baseVersionSource,
                SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time
            };

            var commitLog = context.Repository.Commits.QueryBy(qf);
            var commitsSinceTag = commitLog.Count();
            Logger.WriteInfo(string.Format("{0} commits found between {1} and {2}", commitsSinceTag, baseVersionSource.Sha, context.CurrentCommit.Sha));

            var shortSha = context.Repository.ObjectDatabase.ShortenObjectId(context.CurrentCommit);
            return new SemanticVersionBuildMetaData(
                commitsSinceTag,
                context.CurrentBranch.FriendlyName,
                context.CurrentCommit.Sha,
                shortSha,
                context.CurrentCommit.When());
        }
    }
}