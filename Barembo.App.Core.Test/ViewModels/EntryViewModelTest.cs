using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
using Barembo.Interfaces;
using Barembo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Test.ViewModels
{
    [TestClass]
    public class EntryViewModelTest
    {
        EntryViewModel _viewModel;
        Moq.Mock<IEntryService> _entryServiceMock;
        EntryReference _entryReference;
        Moq.Mock<System.Threading.SynchronizationContext> _syncConMock;

        [TestInitialize]
        public void Init()
        {
            _entryReference = new EntryReference();
            _entryServiceMock = new Moq.Mock<IEntryService>();
            _syncConMock = new Moq.Mock<System.Threading.SynchronizationContext>();
            _viewModel = new EntryViewModel(_entryReference, _entryServiceMock.Object, _syncConMock.Object);
        }

        [TestMethod]
        public void IfNotLoaded_Shows_Placeholders()
        {
            Assert.AreEqual("...", _viewModel.Header);
            Assert.AreEqual("...", _viewModel.Body);
            Assert.IsNull(_viewModel.Thumbnail);
        }

        [TestMethod]
        public void IsLoading_IfPropertiesAreAccessed()
        {
            _entryServiceMock.Setup(s => s.LoadEntryAsSoonAsPossible(_entryReference, Moq.It.IsAny<ElementLoadedDelegate<Entry>>(), Moq.It.IsAny<ElementLoadingDequeuedDelegate>())).Verifiable();

            Assert.IsFalse(_viewModel.IsLoading);

            Assert.AreEqual("...", _viewModel.Header);

            Assert.IsTrue(_viewModel.IsLoading);

            _entryServiceMock.Verify();
        }

        [TestMethod]
        public void InitFromEntry_Refreshes_ViewModelValues()
        {
            Entry entry = new Entry();
            entry.Header = "Header";
            entry.Body = "Body";
            entry.ThumbnailBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Barembo rockz"));

            _viewModel.InitFromEntry(entry);

            Assert.AreEqual(entry.Header, _viewModel.Header);
            Assert.AreEqual(entry.Body, _viewModel.Body);
            Assert.AreEqual("Barembo rockz", Encoding.UTF8.GetString(_viewModel.Thumbnail));

            Assert.IsFalse(_viewModel.IsLoading);
        }

        [TestMethod]
        public void AccessingProperties_LoadsEntry_OnlyOnce()
        {
            Entry entry = new Entry();
            entry.Header = "Header";
            entry.Body = "Body";

            _viewModel.InitFromEntry(entry);

            var header = _viewModel.Header; //Access property

            Assert.IsFalse(_viewModel.IsLoading);
            _entryServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task LoadEntryAsync_LoadsEntry()
        {
            Entry entry = new Entry();
            entry.Header = "Header";
            entry.Body = "Body";

            _entryServiceMock.Setup(s => s.LoadEntryAsync(_entryReference)).Returns(Task.FromResult(entry)).Verifiable();
            _syncConMock.Setup(s => s.Post(Moq.It.IsAny<System.Threading.SendOrPostCallback>(), null)).Callback(() => { _viewModel.InitFromEntry(entry);  });

            await _viewModel.LoadEntryAsync();

            Assert.IsFalse(_viewModel.IsLoading);
            _entryServiceMock.Verify();
            _syncConMock.Verify();
        }

        [TestMethod]
        public async Task LoadEntryAsync_SetsHasThumbnail_IfThumbnailExists()
        {
            Entry entry = new Entry();
            entry.Header = "Header";
            entry.Body = "Body";
            entry.ThumbnailBase64 = "thumbnailBAse64";

            _entryServiceMock.Setup(s => s.LoadEntryAsync(_entryReference)).Returns(Task.FromResult(entry)).Verifiable();
            _syncConMock.Setup(s => s.Post(Moq.It.IsAny<System.Threading.SendOrPostCallback>(), null)).Callback(() => { _viewModel.InitFromEntry(entry); });

            await _viewModel.LoadEntryAsync();

            Assert.IsTrue(_viewModel.HasThumbnail);
            _entryServiceMock.Verify();
            _syncConMock.Verify();
        }

        [TestMethod]
        public async Task LoadEntryAsync_LeavesHasThumbnail_IfThumbnailDoesNotExists()
        {
            Entry entry = new Entry();
            entry.Header = "Header";
            entry.Body = "Body";
            entry.ThumbnailBase64 = string.Empty;

            _entryServiceMock.Setup(s => s.LoadEntryAsync(_entryReference)).Returns(Task.FromResult(entry)).Verifiable();
            _syncConMock.Setup(s => s.Post(Moq.It.IsAny<System.Threading.SendOrPostCallback>(), null)).Callback(() => { _viewModel.InitFromEntry(entry); });

            await _viewModel.LoadEntryAsync();

            Assert.IsFalse(_viewModel.HasThumbnail);
            _entryServiceMock.Verify();
            _syncConMock.Verify();
        }

        [TestMethod]
        public async Task LoadingAttachmentPreviews_Loads_AllPreviews()
        {
            Attachment attachment = new Attachment();
            AttachmentPreview preview = new AttachmentPreview();
            Entry entry = new Entry();
            entry.Header = "Header";
            entry.Body = "Body";
            entry.Attachments.Add(attachment);

            _entryServiceMock.Setup(s => s.LoadAttachmentPreviewAsync(_entryReference, attachment)).Returns(Task.FromResult(preview)).Verifiable();
            _viewModel.InitFromEntry(entry);

            await _viewModel.LoadAttachmentPreviewsAsync();

            Assert.AreEqual(1, _viewModel.AttachmentPreviews.Count);
            Assert.AreEqual(preview, _viewModel.AttachmentPreviews[0]._attachmentPreview);
            _entryServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadingAttachmentPreviews_DoesNothing_IfEntryIsNotLoadedYet()
        {
            await _viewModel.LoadAttachmentPreviewsAsync();

            _entryServiceMock.VerifyNoOtherCalls();
        }
    }
}
