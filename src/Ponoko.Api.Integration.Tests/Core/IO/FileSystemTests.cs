﻿using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Core.IO;

namespace Ponoko.Api.Integration.Tests.Core.IO {
	[TestFixture]
	public class FileSystemTests {
		[Test]
		public void it_creates_a_file_on_disk() {
			var theFilePath = NewRandomTempFile();
			
			Assert.IsFalse(File.Exists(theFilePath), "Invalid test data, the file must not exist before it is created!");
			
			new DefaultFileSystem().New(theFilePath);

			Assert.IsTrue(File.Exists(theFilePath), "Expected the file to have been created on disk");
		}

		[Test]
		public void it_can_open_any_file_for_read_and_write_and_seek() {
			var fileSystem = new DefaultFileSystem();

			var anyFile = fileSystem.New(NewRandomTempFile());
			
			using (var theStream = fileSystem.Open(anyFile)) {
				Assert.IsTrue(theStream.CanRead	, "Expected the resultant stream to be readable");
				Assert.IsTrue(theStream.CanWrite, "Expected the resultant stream to be writable");
				Assert.IsTrue(theStream.CanSeek	, "Expected the resultant stream to be seekable");
			}
		}

		[Test]
		public void it_opens_the_file_supplied() {
			var fileSystem = new DefaultFileSystem();

			var anyFile = fileSystem.New(NewRandomTempFile());

			var theExpectedText = "Let’s make toast the American way, you burn it and I’ll scrape it.";

			using (var theStream = fileSystem.Open(anyFile)) {
				using (var writer = new StreamWriter(theStream)) {
					writer.Write(theExpectedText);
				}
			}

			var theActualTextInTheFile = File.ReadAllText(anyFile.FullName);

			Assert.AreEqual(theExpectedText, theActualTextInTheFile, 
				"Expected the text in the file <{0}> to match because we asked DefaultFileSystem to " + 
				"open it and then we wrote that text to it. " + 
				"This means the file we asked to be opened was not opened (because the write was successful).", 
				anyFile
			);
		}

		[Test] 
		public void it_does_not_create_the_file_if_it_does_not_exist() {
			var fileSystem = new DefaultFileSystem();

			var aFileThatDoesNotExist = new FileInfo("xxx.xxx");
			
			File.Delete(aFileThatDoesNotExist.FullName);

			Assert.IsFalse(aFileThatDoesNotExist.Exists);

			var theError = Assert.Throws<FileNotFoundException>(() => fileSystem.Open(aFileThatDoesNotExist));

			Assert.That(theError.Message, Is.StringMatching("^Could not find file"));
		}

		[Test]
		public void it_deletes_files() {
			var theFilePath = new FileInfo(NewRandomTempFile());

			using (File.Create(theFilePath.FullName)) {;}

			Assert.IsTrue(theFilePath.Exists, "Invalid test data. Expected the file to exist so it can be deleted.");

			var fileSystem = new DefaultFileSystem();

			fileSystem.Delete(theFilePath);

			Assert.IsTrue(theFilePath.Exists, "Expected the file to have been deleted");
		}

		private String NewRandomTempFile() {
			return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
		}
	}

}