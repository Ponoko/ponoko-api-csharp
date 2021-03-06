﻿using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Ponoko.Api.Core.IO;

namespace Ponoko.Api.Rest.Integration.Tests.IO {
	[TestFixture]
	public class DefaultFileSystemTests {
		[Test]
		public void it_creates_a_file_on_disk() {
			var theFilePath = NewRandomTempFile();
			
			Assert.IsFalse(File.Exists(theFilePath), 
				"Invalid test data, The file <{0}> already exists. " + 
				"The file must NOT exist before a create attempt is tried.", 
				theFilePath
			);
			
			new DefaultFileSystem().New(theFilePath);

			Assert.IsTrue(File.Exists(theFilePath), "Expected the file to have been created on disk");
		}

		[Test]
		public void it_fails_to_create_a_new_file_if_the_file_already_exists() {
			var theFilePath = NewRandomTempFile();

			var fileSystem = new DefaultFileSystem();
			
			fileSystem.New(theFilePath);

			Assert.IsTrue(File.Exists(theFilePath), 
				"Invalid test data, The file <{0}> does not exist. " + 
				"The file must exist before a create attempt is tried.", 
				theFilePath
			);

			Assert.Throws<IOException>(() => fileSystem.New(theFilePath), 
				"Expected trying to create a file that already exists to fail with error"
			);
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

		[Test]
		public void you_need_to_flush_a_file_stream_to_propagate_bytes_to_disk_unless_buffer_is_full() {
			var fileSystem = new DefaultFileSystem();
			var theFilePath = fileSystem.New(NewRandomTempFile());

			using (var _out = fileSystem.Open(theFilePath)) {
				var bytes = Encoding.UTF8.GetBytes("An example string");
				_out.Write(bytes, 0, bytes.Length);
				
				Assert.That(theFilePath.Length, Is.EqualTo(0), 
					"Expected the bytes to have been buffered and not yet written to disk"
				);
				
				_out.Flush();

				theFilePath.Refresh();
				
				Assert.That(theFilePath.Length, Is.GreaterThan(0), 
					"Expected the bytes to have been written to disk now that the stream has been flushed"
				);
			}
		}

		[Test]
		public void it_has_a_buffer_size_of_1024_bytes() {
			AssertDoesNotFlushAutomatically(	new Byte[0]);
			AssertDoesNotFlushAutomatically(	new Byte[1023]);
			AssertFlushesAutomatically(			new Byte[1024]);
			AssertFlushesAutomatically(			new Byte[1025]);
		}

		private void AssertDoesNotFlushAutomatically(Byte[] bytes) {
			var fileSystem = new DefaultFileSystem();
			var theFilePath = fileSystem.New(NewRandomTempFile());
			
			Assert.That(theFilePath.Length, Is.EqualTo(0), 
				"Expected the file to be empty to start with"
			);

			using (var _out = fileSystem.Open(theFilePath)) {
				_out.Write(bytes, 0, bytes.Length);
				
				theFilePath.Refresh();
				
				Assert.AreEqual(0, theFilePath.Length, 
					"Expected that the bytes would not be flushed because the buffer is not yet full"
				);
			}
		}

		private void AssertFlushesAutomatically(Byte[] bytes) {
			var fileSystem = new DefaultFileSystem();
			var theFilePath = fileSystem.New(NewRandomTempFile());
			
			Assert.That(theFilePath.Length, Is.EqualTo(0), 
				"Expected the file to be empty to start with"
			);

			using (var _out = fileSystem.Open(theFilePath)) {
				_out.Write(bytes, 0, bytes.Length);
				
				theFilePath.Refresh();
				
				Assert.AreEqual(bytes.Length, theFilePath.Length, 
					"Expected the bytes to have been automatically flushed because the buffer was filled"
				);
			}
		}

		private String NewRandomTempFile() {
			return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
		}
	}

}
