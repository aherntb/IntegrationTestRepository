using NUnit.Framework;
using System;
using System.IO;

namespace IntegrationService.IntegrationTests
{
    [SetUpFixture]
    public class DatabaseSetup : DatabaseSetupBase
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            SetupFixture();
        }

        [OneTimeTearDown]
        public void RunAfterAllTests()
        {
            //base.TeardownFixture();
        }

        internal override void PrepareDatabase()
        {
            const string prodental = "ProdentalDatabase";

            var directory = TestContext.CurrentContext.TestDirectory;

            var branchInfo = GetBranchLocation(directory);

            //var branchName = branchInfo.Name;

            //TODO: if scripts fail check the branch/folder values
            var procedures = Path.Combine(branchInfo.FullName, prodental, "Procedures", "MemberAssignment");
            var triggers = Path.Combine(branchInfo.FullName, prodental, "Triggers");
            var scripts = Path.Combine(branchInfo.FullName, prodental, "Scripts", "VBP2019");

            RunSplitScript(Path.Combine(directory, "WW1Create.sql"));
            RunSplitScript(Path.Combine(directory, "WW1References.sql"));

            RunSplitScript(Path.Combine(scripts, "0003444_COMMON_N9_PCD_Auto_Assign_Log_Tables.sql"));

            RunSplitScript(Path.Combine(procedures, "dbo.prDodMaPCDAss_GetActivePCDRules.prc"));
            RunSplitScript(Path.Combine(procedures, "dbo.prDodMaPCDAss_GetMemberPCDRule.prc"));
            RunSplitScript(Path.Combine(procedures, "dbo.prDodMaPCDAutoAssignment_Log_Success.sql"));
            RunSplitScript(Path.Combine(procedures, "dbo.prDodMaPCDAutoAssignment_Log_Delete.sql"));
            RunSplitScript(Path.Combine(procedures, "dbo.prDodMaPCDAutoAssignment_Log_Error_Message.sql"));
            RunSplitScript(Path.Combine(procedures, "dbo.prDodMaPCDAutoAssignment_Create_Log_Detail.sql"));
            RunSplitScript(Path.Combine(procedures, "dbo.prDodMaPCDAutoAssignment_Create_Log.sql"));
            RunSplitScript(Path.Combine(procedures, "dbo.prDodMaPCDAutoAssignment_Get_Log.sql"));
            RunSplitScript(Path.Combine(procedures, "dbo.prDodMaPCDAutoAssignment_Get_Log_Detail.sql"));
            RunSplitScript(Path.Combine(triggers, "dbo.trGlobalParamsAAHistoricalAssignmentDaysUpdate.TRG"));
            //TODO: run here your alter scripts and any other scripts to get the db into state for testing new code
        }

        internal static DirectoryInfo GetBranchLocation(string directory)
        {
            var current = Directory.GetParent(directory);
            while (current != current.Root && (current.Parent.Name.ToUpperInvariant() != "FEATURES" && current.Parent.Name.ToUpperInvariant() != "TS_PROJECT"))
            {
                current = current.Parent;
            }

            if (current == current.Root)
            {
                throw new Exception("Could not locate feature branch! " + directory);
            }

            return current;
        }
    }
}
