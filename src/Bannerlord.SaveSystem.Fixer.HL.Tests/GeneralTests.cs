using Bannerlord.SaveSystem.Tests.Extensions;

using NUnit.Framework;

using System;
using System.IO;

using TaleWorlds.Core;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace Bannerlord.SaveSystem.Tests
{
    public class GeneralTests
    {
        private SubModule? _subModule;

        [SetUp]
        public void Setup()
        {
            _subModule = new SubModule();
            _subModule.Load();
        }

        [Test]
        public void Load_Test()
        {
            ISaveDriver saveDriver;
            LoadResult loadResult;

            foreach (var saveFile in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves"), "*.sav", SearchOption.AllDirectories))
            {
                saveDriver = new FileDriver(saveFile);

                var applicationVersion = saveDriver.LoadMetaData().GetApplicationVersion();
                if (applicationVersion.Major <= 1 && applicationVersion.Minor <= 4 && applicationVersion.Revision < 2)
                    saveDriver = new OldFileDriver(saveFile);

                loadResult = SaveManager.Load(saveDriver, true);
                Assert.True(loadResult.Successful);

                saveDriver = null!;
                loadResult = null!;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        [Test]
        public void Load_Initialize_Test()
        {
            ISaveDriver saveDriver;
            LoadResult loadResult;

            foreach (var saveFile in Directory.GetFiles(Path.Combine("D:\\GitHub\\Bannerlord.SaveSystem.Fixer\\resources", "Saves"), "*.sav", SearchOption.AllDirectories))
            {
                saveDriver = new FileDriver(saveFile);

                var applicationVersion = saveDriver.LoadMetaData().GetApplicationVersion();
                if (applicationVersion.Major <= 1 && applicationVersion.Minor <= 4 && applicationVersion.Revision < 2)
                    saveDriver = new OldFileDriver(saveFile);

                loadResult = SaveManager.Load(saveDriver, true);
                loadResult.InitializeObjects();

                saveDriver = null!;
                loadResult = null!;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}