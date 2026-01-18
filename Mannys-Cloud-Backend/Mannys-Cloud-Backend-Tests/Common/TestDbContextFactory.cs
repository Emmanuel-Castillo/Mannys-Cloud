using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileModel = Mannys_Cloud_Backend.Models.File;

namespace Mannys_Cloud_Backend_Tests.Common
{
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext CreateEmptyDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            return context;
        }

        public static ApplicationDbContext CreateSeededDb()
        {
            var context = CreateEmptyDb();

            context.Users.AddRange([
                new User {FullName = "testuser", Email = "testuser@gmail.com", PasswordHash = "testpw"}
                ]);

            context.Folders.AddRange([
                new Folder {
                    FolderId = 1,
                    FolderName = "root",
                    UserId = 1,
                    IsRootFolder = true,
                },
                new Folder {
                    FolderId = 2,
                    FolderName = "childFolder1",
                    UserId = 1,
                    ParentFolderId = 1
                },
                new Folder {
                    FolderId = 3,
                    FolderName = "childFolder2",
                    UserId = 1,
                    ParentFolderId = 2
                },
                new Folder {
                    FolderId = 4,
                    FolderName = "childFolder3",
                    UserId = 1,
                    ParentFolderId = 1
                },
                new Folder {
                    FolderId = 5,
                    FolderName = "childFolder4",
                    UserId = 1,
                    ParentFolderId = 1,
                    IsDeleted = true
                }
                ]);

            context.Files.AddRange([new FileModel
            {
                UserId = 1,
                BlobPath = "",
                ContentType = "text/plain",
                FileName = "test.txt",
                FolderId = 1,
                SizeBytes = 1024,
            }, 
            new FileModel {
                UserId = 1,
                BlobPath = "",
                ContentType = "text/plain",
                FileName = "test2.txt",
                FolderId = 1,
                SizeBytes = 1024,
            },
            new FileModel {
                UserId = 1,
                BlobPath = "",
                ContentType = "text/plain",
                FileName = "test3.txt",
                FolderId = 1,
                SizeBytes = 1024,
                IsDeleted = true,
            },
            new FileModel {
                UserId = 1,
                BlobPath = "",
                ContentType = "text/plain",
                FileName = "testFileinChildFolder.txt",
                FolderId = 2,
                SizeBytes = 1024,
            },
            new FileModel {
                UserId = 1,
                BlobPath = "",
                ContentType = "text/plain",
                FileName = "testFileinChildFolder2.txt",
                FolderId = 3,
                SizeBytes = 1024,
            },
            ]);

            

            context.SaveChanges();

            return context;
        }
    }
}
