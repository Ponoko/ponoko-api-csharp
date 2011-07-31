using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Rest.Mime;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Rest.Mime {
	[TestFixture]
	public class BodyTests {
		[Test]
		public void it_closes_its_underlying_stream() {
			var fakeStore = MockRepository.GenerateMock<Stream>();
			var body = new Body(fakeStore);

			body.Dispose();

			fakeStore.AssertWasCalled(it => it.Close(), options => options.
				Repeat.Once().
				Message("Expected that when a body is disposed of, it closes its underlying stream")
			);
		}
	}

	[TestFixture]
	public class TempfileTests {
		[Test]
		public void it_deletes_its_underlying_file_when_it_is_disposed() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			var tempFile = new Tempfile(fakeFileSystem);

			tempFile.Dispose();

			fakeFileSystem.AssertWasCalled(
				it => it.Delete(Arg<FileInfo>.Is.Anything), 
				options => options.Repeat.Once().Message("Expected that when I close a TempFile, the underlying file is removed.")
			);
		}

		[Test]
		public void it_deletes_the_same_file_it_creates() {
			var theUnderlyingFile = new FileInfo("xxx_any_fake_file_name_xxx");
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			
			fakeFileSystem.Stub(it => it.New(Arg<String>.Is.Anything)).Return(theUnderlyingFile);

			var tempFile = new Tempfile(fakeFileSystem);
			tempFile.Dispose();

			fakeFileSystem.AssertWasCalled(
				it => it.Delete(Arg<FileInfo>.Is.Same(theUnderlyingFile)), 
				options => options.Repeat.Once().Message(
					"Expected that when it deletes its underlying file, " + 
					"it is the same one that was returned by the file system at the beginning"
				)
			);
		}

		[Test]
		public void it_creates_a_file_once() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			
			new Tempfile(fakeFileSystem);

			fakeFileSystem.AssertWasCalled(
				it => it.New(Arg<String>.Is.Anything), 
				options => options.Repeat.Once().Message("Expected a new file to be created when a Temfile is initialized")
			);
		}

		[Test]
		public void it_creates_files_in_the_system_temp_file_directory() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			
			new Tempfile(fakeFileSystem);

			var theArgs = fakeFileSystem.GetArgumentsForCallsMadeOn(it => it.New(Arg<String>.Is.Anything));

			var actualFilename = theArgs[0][0].ToString();

			var theDirectory = Path.GetDirectoryName(actualFilename);

			var expectedDirectory = Path.GetDirectoryName(Path.GetTempPath());

			Assert.AreEqual(expectedDirectory, theDirectory, 
				"Expected the underlying files to all go in the temp directory for the current system"
			);
		}

		[Test] 
		public void it_creates_files_with_unique_non_empty_names() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			
			new Tempfile(fakeFileSystem);

			var theArgs = fakeFileSystem.GetArgumentsForCallsMadeOn(it => it.New(Arg<String>.Is.Anything));

			var theFirstFileNameUsed = theArgs[0][0];

			fakeFileSystem = MockRepository.GenerateMock<FileSystem>();

			new Tempfile(fakeFileSystem);

			theArgs = fakeFileSystem.GetArgumentsForCallsMadeOn(it => it.New(Arg<String>.Is.Anything));

			var theSecondFileNameUsed = theArgs[0][0];

			Assert.AreNotEqual(String.Empty, theFirstFileNameUsed, "Tempfile must not create files with empty names");
			Assert.AreNotEqual(String.Empty, theSecondFileNameUsed, "Tempfile must not create files with empty names");
			Assert.AreNotEqual(theFirstFileNameUsed, theSecondFileNameUsed, "Expected every new Tempfile instance to create a different file");
		}
	}

	public class Tempfile : IDisposable {
		private readonly FileSystem _fileSystem;
		private FileInfo File { get; set; }
		
		public Tempfile(FileSystem fileSystem) {
			_fileSystem = fileSystem;
			var tempDir = Path.GetTempPath();
			var randomFileName = Path.GetRandomFileName();
			var fullPath = Path.Combine(tempDir, randomFileName);

			File = fileSystem.New(fullPath);
		}

		//public Int64 Write(Byte[] buffer, Int32 offset, Int32 length) {}

		public void Dispose() {
			_fileSystem.Delete(File);
		}
	}

	public interface FileSystem {
		FileInfo New(String filename);
		void Delete(FileInfo filename);
	}
}
