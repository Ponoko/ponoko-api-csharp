﻿using System;
using System.IO;
using System.Text;
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

			var actualDirectoryuPath = Path.GetDirectoryName(actualFilename);

			var expectedDirectoryName = Path.GetDirectoryName(Path.GetTempPath());

			Assert.AreEqual(expectedDirectoryName, actualDirectoryuPath,
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

		[Test]
		public void it_opens_the_file_the_first_time_you_write_to_it() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			var fakeFileStream = NewFakeFileStream();

			var tempFile = new Tempfile(fakeFileSystem);

			fakeFileSystem.
				Expect(it => it.Open(Arg<FileInfo>.Is.Anything)).
				Repeat.Once().
				Return(fakeFileStream).
				Message("Expected the underlying file to have been opened");

			var anyBytes = Encoding.UTF8.GetBytes("Ben rules");

			tempFile.Write(anyBytes, 0, anyBytes.Length);
			tempFile.Write(anyBytes, 0, anyBytes.Length);
		}

		// TEST: it_closes_the_file_stream_before_deleting_it

		private FileStream NewFakeFileStream() {
			var fakeFileStream = MockRepository.GenerateMock<FileStream>();
			
			fakeFileStream.Stub(it => 
				it.Write(Arg<Byte[]>.Is.Anything, Arg<Int32>.Is.Anything, Arg<Int32>.Is.Anything)
			);
			
			return fakeFileStream;
		}

		public class Tempfile : IDisposable {
			private readonly FileSystem _fileSystem;
			private FileInfo File { get; set; }
			private FileStream _out;

			private FileStream Out {
				get { return _out ?? (_out = _fileSystem.Open(File)); }
			}

			public Tempfile(FileSystem fileSystem) {
				_fileSystem = fileSystem;
				var tempDir = Path.GetTempPath();
				var randomFileName = Path.GetRandomFileName();
				var fullPath = Path.Combine(tempDir, randomFileName);

				File = fileSystem.New(fullPath);
			}

			public void Write(Byte[] buffer, Int32 offset, Int32 count) {
				Out.Write(buffer, offset, count);
			}

			public void Dispose() {
				_fileSystem.Delete(File);
			}
		}

		public interface FileSystem {
			FileInfo New(String filename);
			FileStream Open(FileInfo filename);
			void Delete(FileInfo filename);
		}
	}
}
