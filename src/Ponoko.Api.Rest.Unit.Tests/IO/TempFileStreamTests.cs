﻿using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Ponoko.Api.Core.IO;
using Rhino.Mocks;

namespace Ponoko.Api.Rest.Unit.Tests.IO {
	[TestFixture]
	public class TempFileStreamTests {
		[Test]
		public void it_creates_its_underlying_file_on_disk_exactly_once() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();

			new TempFileStream(fakeFileSystem);

			fakeFileSystem.AssertWasCalled(
				it => it.New(Arg<String>.Is.Anything),
				options => options.Repeat.Once().Message("Expected a new file to be created when a Temfile is initialized")
			);
		}

		[Test]
		public void it_creates_its_underlying_file_in_the_system_temp_file_directory() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();

			new TempFileStream(fakeFileSystem);

			var theArgs = fakeFileSystem.GetArgumentsForCallsMadeOn(it => it.New(Arg<String>.Is.Anything));

			var actualFilename = theArgs[0][0].ToString();

			var actualDirectoryPath = Path.GetDirectoryName(actualFilename);

			var expectedDirectoryName = Path.GetDirectoryName(Path.GetTempPath());

			Assert.AreEqual(expectedDirectoryName, actualDirectoryPath,
				"Expected the underlying files to all go in the temp directory for the current system"
			);
		}

		[Test]
		public void it_creates_its_underlying_file_with_a_unique_non_empty_name() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();

			new TempFileStream(fakeFileSystem);

			var theArgs = fakeFileSystem.GetArgumentsForCallsMadeOn(it => it.New(Arg<String>.Is.Anything));

			var theFirstFileNameUsed = theArgs[0][0];

			fakeFileSystem = MockRepository.GenerateMock<FileSystem>();

			new TempFileStream(fakeFileSystem);

			theArgs = fakeFileSystem.GetArgumentsForCallsMadeOn(it => it.New(Arg<String>.Is.Anything));

			var theSecondFileNameUsed = theArgs[0][0];

			Assert.AreNotEqual(String.Empty, theFirstFileNameUsed, 
				"TempFileStream must not create files with empty names"
			);
			Assert.AreNotEqual(String.Empty, theSecondFileNameUsed, 
				"TempFileStream must not create files with empty names"
			);
			Assert.AreNotEqual(theFirstFileNameUsed, theSecondFileNameUsed, 
				"Expected every new TempFileStream instance to create a different file"
			);
		}

		[Test]
		public void it_opens_its_underlying_file_the_first_time_you_write_to_it() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			var fakeFileStream = NewFakeFileStream();

			fakeFileSystem.
				Expect(it => it.Open(Arg<FileInfo>.Is.Anything)).
				Return(fakeFileStream).
				Repeat.Once(); // [!] Does not work, try setting it to Twice() and it doesn't fail

			var tempFile = new TempFileStream(fakeFileSystem);

			var anyBytes = Encoding.UTF8.GetBytes("Ben rules");

			tempFile.Write(anyBytes, 0, anyBytes.Length);
			tempFile.Write(anyBytes, 0, anyBytes.Length);
		}

		[Test]
		public void it_writes_to_its_underlying_file() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			var fakeFileStream = NewFakeFileStream();

			fakeFileSystem.
				Stub(it => it.Open(Arg<FileInfo>.Is.Anything)).
				Return(fakeFileStream);

			var tempFile = new TempFileStream(fakeFileSystem);

			var lineOne = Encoding.UTF8.GetBytes("Lasciate ogne speranza, voi ch'intrate");
			var lineTwo = Encoding.UTF8.GetBytes("Abandon all hope, ye who enter here");

			tempFile.Write(lineOne, 0, lineOne.Length);
			tempFile.Write(lineTwo, 0, lineOne.Length);

			var allTheArgs = fakeFileStream.GetArgumentsForCallsMadeOn(it =>
				it.Write(
					Arg<Byte[]>.Is.Anything,
			        Arg<Int32>.Is.Anything,
			        Arg<Int32>.Is.Anything
				)
			);

			var firstWrittenLine = Encoding.UTF8.GetString((Byte[])allTheArgs[0][0]);
			var secondWrittenLine = Encoding.UTF8.GetString((Byte[])allTheArgs[1][0]);

			Assert.AreEqual(Encoding.UTF8.GetString(lineOne), firstWrittenLine);
			Assert.AreEqual(Encoding.UTF8.GetString(lineTwo), secondWrittenLine);
		}

		[Test]
		public void it_deletes_its_underlying_file_when_it_is_disposed() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			var tempFile = new TempFileStream(fakeFileSystem);

			tempFile.Dispose();

			fakeFileSystem.AssertWasCalled(
				it => it.Delete(Arg<FileInfo>.Is.Anything),
				options => options.Repeat.Once().Message("Expected that when I close a TempFile, the underlying file is deleted.")
			);
		}

		[Test]
		public void it_deletes_the_same_file_it_creates() {
			var theUnderlyingFile = new FileInfo("xxx_any_fake_file_name_xxx");
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();

			fakeFileSystem.Stub(it => it.New(Arg<String>.Is.Anything)).Return(theUnderlyingFile);

			var tempFile = new TempFileStream(fakeFileSystem);
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
		public void it_seeks_on_its_underlying_file() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			var fakeFileStream = NewFakeFileStream();

			fakeFileSystem.
				Stub(it => it.Open(Arg<FileInfo>.Is.Anything)).
				Return(fakeFileStream);

			var tempFile = new TempFileStream(fakeFileSystem);

			var expectedOffset = 1337;
			var expectedSeekOrigin = SeekOrigin.Begin;

			tempFile.Seek(expectedOffset, expectedSeekOrigin);

			fakeFileStream.AssertWasCalled(
				it => it.Seek(expectedOffset, expectedSeekOrigin), 
				options => options.Message("Expected it to seek from the supplied offset using the supplied seek origin")
			);
		}

		[Test]
		public void it_closes_its_file_stream_before_deleting_the_file() {
			var theUnderlyingFile = new FileInfo("xxx_any_fake_file_name_xxx");
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			var fakeFileStream = NewFakeFileStream();
			var itHasDeletedTheFile = false;

			fakeFileSystem.Stub(it => it.New(Arg<String>.Is.Anything)).Return(theUnderlyingFile);

			fakeFileSystem.
				Expect(it => it.Delete(Arg<FileInfo>.Is.Anything)).
				Repeat.Once().Message("Expected it to delete its underlying file").
				Callback((FileInfo file) => { itHasDeletedTheFile = true; return true; });

			fakeFileStream.Expect(it => it.Close()).Repeat.Once().
				Callback(() => { Assert.IsFalse(itHasDeletedTheFile, "Expected it to close the stream BEFORE deleting the file"); return true; }).
				Message("Expected it to close its file stream");

			fakeFileSystem.
				Stub(it => it.Open(Arg<FileInfo>.Is.Anything)).
				Return(fakeFileStream);

			var tempFile = new TempFileStream(fakeFileSystem);
			tempFile.Write(new Byte[0], 0, 0);
			tempFile.Dispose();
		}

		[Test]
		public void it_skips_closing_the_file_stream_if_nothing_has_been_written() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			var fakeFileStream = NewFakeFileStream();

			fakeFileSystem.Stub(it => it.New(Arg<String>.Is.Anything)).
				Return(new FileInfo("xxx_any_fake_file_name_xxx"));
			
			fakeFileSystem.Stub(it => it.Delete(Arg<FileInfo>.Is.Anything));

			fakeFileSystem.Stub(it => it.Open(Arg<FileInfo>.Is.Anything)).
				Return(fakeFileStream);

			var tempFile = new TempFileStream(fakeFileSystem);
			tempFile.Dispose();

			fakeFileStream.AssertWasNotCalled(it => it.Close(), options => options.Message(
				"Expected it to skip closing the stream because it is only initialized on the first write"
			));
		}

		[Test]
		public void it_reads_from_its_underlying_file() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			var fakeFileStream = NewFakeFileStream();

			fakeFileSystem.
				Stub(it => it.Open(Arg<FileInfo>.Is.Anything)).
				Return(fakeFileStream);

			var tempFile = new TempFileStream(fakeFileSystem);

			var buffer = new Byte[32];

			tempFile.Read(buffer, 0, 32);

			fakeFileStream.AssertWasCalled(
				it => it.Read(buffer, 0, 32), 
				options => options.Message("Expected it to read 32 bytes from its underlying stream")
			);
		}

		[Test]
		public void it_does_not_support_reading_or_writing_its_length() {
			var fakeFileSystem = MockRepository.GenerateMock<FileSystem>();
			var tempFile = new TempFileStream(fakeFileSystem);

			Assert.Throws<NotImplementedException>(() => tempFile.SetLength(1337));
			Assert.Throws<NotImplementedException>(() => { var length = tempFile.Length; });
		}
		// TEST: it_does_not_open_file_if_nothing_written

		private FileStream NewFakeFileStream() {
			var fakeFileStream = MockRepository.GenerateMock<FileStream>();

			fakeFileStream.Stub(it =>
				it.Write(Arg<Byte[]>.Is.Anything, Arg<Int32>.Is.Anything, Arg<Int32>.Is.Anything)
			);

			return fakeFileStream;
		}
	}
}