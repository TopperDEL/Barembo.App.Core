﻿using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
using Barembo.Interfaces;
using Barembo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Test.ViewModels
{
    [TestClass]
    public class CreateBookEntryViewModelTest
    {
        CreateBookEntryViewModel _viewModel;
        Moq.Mock<IEntryService> _entryServiceMock;
        Moq.Mock<IEventAggregator> _eventAggregator;

        [TestInitialize]
        public void Init()
        {
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _entryServiceMock = new Moq.Mock<IEntryService>();
            _viewModel = new CreateBookEntryViewModel(_entryServiceMock.Object, _eventAggregator.Object);
        }

        [TestMethod]
        public void ExecuteSaveEntry_Publishes_Message()
        {
            BookReference bookReference = new BookReference();
            _eventAggregator.Setup(s => s.GetEvent<BookEntrySavedMessage>()).Returns(new BookEntrySavedMessage()).Verifiable();

            _viewModel.Init(bookReference);
            _viewModel.SaveEntryCommand.Execute();

            _eventAggregator.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_Saves_Entry()
        {
            BookReference bookReference = new BookReference();
            EntryReference entryRef = new EntryReference();
            Entry entry = new Entry();

            _viewModel.Header = "header";
            _viewModel.Body = "body";

            _eventAggregator.Setup(s => s.GetEvent<BookEntrySavedMessage>()).Returns(new BookEntrySavedMessage()).Verifiable();
            _entryServiceMock.Setup(s => s.CreateEntry(_viewModel.Header, _viewModel.Body)).Returns(entry).Verifiable();
            _entryServiceMock.Setup(s => s.AddEntryToBookAsync(bookReference, entry)).Returns(Task.FromResult(entryRef)).Verifiable();

            _viewModel.Init(bookReference);
            _viewModel.SaveEntryCommand.Execute();

            _eventAggregator.Verify();
            _entryServiceMock.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_Saves_Attachments()
        {
            BookReference bookReference = new BookReference();
            EntryReference entryRef = new EntryReference();
            Entry entry = new Entry();

            _viewModel.Header = "header";
            _viewModel.Body = "body";
            MemoryStream attachmentBinary = new MemoryStream(Encoding.UTF8.GetBytes("Barembo rockz"));

            Attachment attachment = new Attachment();
            attachment.FileName = "attachment1.jpg";
            attachment.Type = AttachmentType.Image;
            attachment.Size = attachmentBinary.Length;
            _viewModel.Attachments.Add(new Tuple<Attachment, System.IO.Stream>(attachment, attachmentBinary));

            _eventAggregator.Setup(s => s.GetEvent<BookEntrySavedMessage>()).Returns(new BookEntrySavedMessage()).Verifiable();
            _entryServiceMock.Setup(s => s.CreateEntry(_viewModel.Header, _viewModel.Body)).Returns(entry).Verifiable();
            _entryServiceMock.Setup(s => s.AddAttachmentAsync(entryRef, entry, attachment, attachmentBinary, true)).Returns(Task.FromResult(true)).Verifiable();
            _entryServiceMock.Setup(s => s.AddEntryToBookAsync(bookReference, entry)).Returns(Task.FromResult(entryRef)).Verifiable();

            _viewModel.Init(bookReference);
            _viewModel.SaveEntryCommand.Execute();

            _eventAggregator.Verify();
            _entryServiceMock.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_Saves_AttachmentsWithSecondOneNotSettingThumbnail()
        {
            BookReference bookReference = new BookReference();
            EntryReference entryRef = new EntryReference();
            Entry entry = new Entry();

            _viewModel.Header = "header";
            _viewModel.Body = "body";
            MemoryStream attachmentBinary = new MemoryStream(Encoding.UTF8.GetBytes("Barembo rockz"));

            Attachment attachment1 = new Attachment();
            attachment1.FileName = "attachment1.jpg";
            attachment1.Type = AttachmentType.Image;
            attachment1.Size = attachmentBinary.Length;
            _viewModel.Attachments.Add(new Tuple<Attachment, System.IO.Stream>(attachment1, attachmentBinary));

            Attachment attachment2 = new Attachment();
            attachment2.FileName = "attachment2.jpg";
            attachment2.Type = AttachmentType.Image;
            attachment2.Size = attachmentBinary.Length;
            _viewModel.Attachments.Add(new Tuple<Attachment, System.IO.Stream>(attachment2, attachmentBinary));

            _eventAggregator.Setup(s => s.GetEvent<BookEntrySavedMessage>()).Returns(new BookEntrySavedMessage()).Verifiable();
            _entryServiceMock.Setup(s => s.CreateEntry(_viewModel.Header, _viewModel.Body)).Returns(entry).Verifiable();
            _entryServiceMock.Setup(s => s.AddAttachmentAsync(entryRef, entry, attachment1, attachmentBinary, true)).Returns(Task.FromResult(true)).Verifiable();
            _entryServiceMock.Setup(s => s.AddAttachmentAsync(entryRef, entry, attachment2, attachmentBinary, false)).Returns(Task.FromResult(true)).Verifiable();
            _entryServiceMock.Setup(s => s.AddEntryToBookAsync(bookReference, entry)).Returns(Task.FromResult(entryRef)).Verifiable();

            _viewModel.Init(bookReference);
            _viewModel.SaveEntryCommand.Execute();

            _eventAggregator.Verify();
            _entryServiceMock.Verify();
        }

        [TestMethod]
        public void GoBack_Goes_Back()
        {
            _eventAggregator.Setup(s => s.GetEvent<GoBackMessage>()).Returns(new GoBackMessage()).Verifiable();
            _viewModel.GoBackCommand.Execute();

            _eventAggregator.Verify();
        }
    }
}
