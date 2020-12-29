using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
using Barembo.Exceptions;
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
            Entry entry = new Entry();

            _entryServiceMock.Setup(s => s.CreateEntry(_viewModel.Header, _viewModel.Body)).Returns(entry).Verifiable();
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
            _entryServiceMock.Setup(s => s.AddEntryToBookAsync(bookReference, entry)).Returns(Task.FromResult(entryRef)).Verifiable();
            _entryServiceMock.Setup(s => s.AddAttachmentAsync(entryRef, entry, attachment, attachmentBinary)).Returns(Task.FromResult(true)).Verifiable();
            _entryServiceMock.Setup(s => s.SetThumbnailAsync(entryRef, entry, attachmentBinary)).Returns(Task.FromResult(true)).Verifiable();

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
            MemoryStream attachmentBinary1 = new MemoryStream(Encoding.UTF8.GetBytes("Barembo rockz"));
            MemoryStream attachmentBinary2 = new MemoryStream(Encoding.UTF8.GetBytes("Barembo really rockz"));

            Attachment attachment1 = new Attachment();
            attachment1.FileName = "attachment1.jpg";
            attachment1.Type = AttachmentType.Image;
            attachment1.Size = attachmentBinary1.Length;
            _viewModel.Attachments.Add(new Tuple<Attachment, System.IO.Stream>(attachment1, attachmentBinary1));

            Attachment attachment2 = new Attachment();
            attachment2.FileName = "attachment2.jpg";
            attachment2.Type = AttachmentType.Image;
            attachment2.Size = attachmentBinary2.Length;
            _viewModel.Attachments.Add(new Tuple<Attachment, System.IO.Stream>(attachment2, attachmentBinary2));

            _eventAggregator.Setup(s => s.GetEvent<BookEntrySavedMessage>()).Returns(new BookEntrySavedMessage()).Verifiable();
            _entryServiceMock.Setup(s => s.CreateEntry(_viewModel.Header, _viewModel.Body)).Returns(entry).Verifiable();
            _entryServiceMock.Setup(s => s.AddAttachmentAsync(entryRef, entry, attachment1, attachmentBinary1)).Returns(Task.FromResult(true)).Verifiable();
            _entryServiceMock.Setup(s => s.SetThumbnailAsync(entryRef, entry, attachmentBinary1)).Returns(Task.FromResult(true)).Verifiable();
            _entryServiceMock.Setup(s => s.AddAttachmentAsync(entryRef, entry, attachment2, attachmentBinary2)).Returns(Task.FromResult(true)).Verifiable();
            _entryServiceMock.Setup(s => s.AddEntryToBookAsync(bookReference, entry)).Returns(Task.FromResult(entryRef)).Verifiable();

            _viewModel.Init(bookReference);
            _viewModel.SaveEntryCommand.Execute();

            _eventAggregator.Verify();
            _entryServiceMock.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_RaisesError_IfEntryCouldNotBeCreated()
        {
            BookReference bookReference = new BookReference();
            Entry entry = null;

            _viewModel.Header = "header";
            _viewModel.Body = "body";

            _entryServiceMock.Setup(s => s.CreateEntry(_viewModel.Header, _viewModel.Body)).Returns(entry).Verifiable();
            _eventAggregator.Setup(s => s.GetEvent<ErrorMessage>()).Returns(new ErrorMessage()).Verifiable();

            _viewModel.Init(bookReference);
            _viewModel.SaveEntryCommand.Execute();

            _eventAggregator.Verify();
            _entryServiceMock.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_RaisesError_IfEntryCouldNotBeAddedToBook()
        {
            BookReference bookReference = new BookReference();
            Entry entry = new Entry();

            _viewModel.Header = "header";
            _viewModel.Body = "body";

            _entryServiceMock.Setup(s => s.CreateEntry(_viewModel.Header, _viewModel.Body)).Returns(entry).Verifiable();
            _entryServiceMock.Setup(s => s.AddEntryToBookAsync(bookReference, entry)).Throws(new EntryCouldNotBeSavedException()).Verifiable();
            _eventAggregator.Setup(s => s.GetEvent<ErrorMessage>()).Returns(new ErrorMessage()).Verifiable();

            _viewModel.Init(bookReference);
            _viewModel.SaveEntryCommand.Execute();

            _eventAggregator.Verify();
            _entryServiceMock.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_RaisesError_IfAttachmentCouldNotBeSaved()
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

            _eventAggregator.Setup(s => s.GetEvent<ErrorMessage>()).Returns(new ErrorMessage()).Verifiable();
            _entryServiceMock.Setup(s => s.CreateEntry(_viewModel.Header, _viewModel.Body)).Returns(entry).Verifiable();
            _entryServiceMock.Setup(s => s.AddEntryToBookAsync(bookReference, entry)).Returns(Task.FromResult(entryRef)).Verifiable();
            _entryServiceMock.Setup(s => s.AddAttachmentAsync(entryRef, entry, attachment, attachmentBinary)).Returns(Task.FromResult(false)).Verifiable();

            _viewModel.Init(bookReference);
            _viewModel.SaveEntryCommand.Execute();

            _eventAggregator.Verify();
            _entryServiceMock.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_RaisesError_IfThumbnailCouldNotBeSet()
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

            _eventAggregator.Setup(s => s.GetEvent<ErrorMessage>()).Returns(new ErrorMessage()).Verifiable();
            _entryServiceMock.Setup(s => s.CreateEntry(_viewModel.Header, _viewModel.Body)).Returns(entry).Verifiable();
            _entryServiceMock.Setup(s => s.AddEntryToBookAsync(bookReference, entry)).Returns(Task.FromResult(entryRef)).Verifiable();
            _entryServiceMock.Setup(s => s.AddAttachmentAsync(entryRef, entry, attachment, attachmentBinary)).Returns(Task.FromResult(true)).Verifiable();
            _entryServiceMock.Setup(s => s.SetThumbnailAsync(entryRef, entry, attachmentBinary)).Returns(Task.FromResult(false)).Verifiable();

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

        [TestMethod]
        public void AddMediaCommand_Raises_MediaRequestedMessage()
        {
            _eventAggregator.Setup(s => s.GetEvent<MediaRequestedMessage>()).Returns(new MediaRequestedMessage()).Verifiable();
            _viewModel.AddMediaCommand.Execute();

            _eventAggregator.Verify();
        }

        [TestMethod]
        public void AddMediaCommand_AddsAttachment_IfSelected()
        {
            var attachment = new Attachment();
            attachment.FileName = "test.jpg";
            var stream = new MemoryStream();
            var message = new Moq.Mock<MediaRequestedMessage>();

            _eventAggregator.Setup(s => s.GetEvent<MediaRequestedMessage>()).Returns(message.Object).Verifiable();
            message.Setup(s => s.Publish(Moq.It.IsAny<MediaResult>()))
                .Callback<MediaResult>(e =>
                {
                    e.MediaSelected = true;
                    e.Attachment = attachment;
                    e.Stream = stream;
                }).Verifiable();

            _viewModel.AddMediaCommand.Execute();
            Assert.AreEqual(attachment,_viewModel.Attachments[0].Item1);
            Assert.AreEqual(stream,_viewModel.Attachments[0].Item2);

            _eventAggregator.Verify();
        }

        [TestMethod]
        public void AddMediaCommand_DoesNothing_IfNotSelected()
        {
            var message = new Moq.Mock<MediaRequestedMessage>();

            _eventAggregator.Setup(s => s.GetEvent<MediaRequestedMessage>()).Returns(message.Object).Verifiable();
            message.Setup(s => s.Publish(Moq.It.IsAny<MediaResult>()))
                .Callback<MediaResult>(e =>
                {
                    e.MediaSelected = false;
                }).Verifiable();

            _viewModel.AddMediaCommand.Execute();
            Assert.AreEqual(0, _viewModel.Attachments.Count);

            _eventAggregator.Verify();
        }
    }
}
