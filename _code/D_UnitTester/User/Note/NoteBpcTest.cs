using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AICPA.Destroyer.Content;
using AICPA.Destroyer.Shared;
using NUnit.Framework;

namespace AICPA.Destroyer.User.Note
{
    public class NoteBpcTest : ContentShared
    {
    }

    [TestFixture]
    public class NoteGeneral : NoteBpcTest
    {
        [Test]
        public void NoteGeneral_Save_and_Retrieve_Note()
        {
            string noteText = "This is a test note text.";
            string targetDoc = "ps";
            string targetPtr = "ar_80_effective_date";
            string title = "title";
            Guid userId = Guid.NewGuid();
            string myDomain = "test-Subscription1" + DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR + "test-Subscription3";
            C2bUser c2bUser = CreateC2bWebServiceUser(userId, myDomain);
            string sessionId = Guid.NewGuid().ToString();
            var cscUser = new User(userId, ReferringSite.Csc);

            try
            {
                //user login
                cscUser.LogOn(sessionId, myDomain);

                INote note = new Note(targetDoc, targetPtr, userId, noteText, title);
                note.Save();

                // Make sure NoteId comes back populated after Save call and check other properties on note object
                Assert.AreNotEqual(0, note.Id, "Calling Save method on a Note was expected to bring back a valid NoteId value from DB.");
                Assert.AreEqual(userId, note.UserId, "Expected UserId to be set on Note object.");
                Assert.AreEqual(targetDoc, note.TargetDoc, "Expected TargetDoc to be set on Note object.");
                Assert.AreEqual(targetPtr, note.TargetPtr, "Expected TargetPtr to be set on Note object.");
                Assert.AreEqual(noteText, note.Text, "Expected Text to be set on Note object.");
                Assert.AreNotEqual(DateTime.MinValue, note.LastModified, "Expected LastModified to be set on Note object.");

                // Test changing some text in the note
                int oldId = note.Id;
                string someNewText = "Some new test text!";
                note.Text = someNewText;
                note.Save();

                Assert.AreEqual(oldId, note.Id, "Expected NoteId from database not to change.");
                Assert.AreEqual(someNewText, note.Text, "Expected Text to have been updated to new value.");

                // Test retrieving note by Id
                INote retrievedNote = Note.GetNoteById(note.Id);

                Assert.AreEqual(note.Id, retrievedNote.Id, "Retrieved NoteId didn't match value persisted.");
                Assert.AreEqual(note.UserId, retrievedNote.UserId, "Retrieved UserId didn't match value persisted.");
                Assert.AreEqual(note.TargetDoc, retrievedNote.TargetDoc, "Retrieved TargetDoc didn't match value persisted.");
                Assert.AreEqual(note.TargetPtr, retrievedNote.TargetPtr, "Retrieved TargetPtr didn't match value persisted.");
                Assert.AreEqual(note.Text, retrievedNote.Text, "Retrieved NoteText didn't match value persisted.");

                // Test retrieving note by userId and targetdoc/ptr
                //retrievedNote = Note.GetNotesByUserIdAndTargetDocPtr(userId, targetDoc, targetPtr);

                Assert.AreEqual(note.Id, retrievedNote.Id, "Retrieved NoteId didn't match value persisted.");
                Assert.AreEqual(note.UserId, retrievedNote.UserId, "Retrieved UserId didn't match value persisted.");
                Assert.AreEqual(note.TargetDoc, retrievedNote.TargetDoc, "Retrieved TargetDoc didn't match value persisted.");
                Assert.AreEqual(note.TargetPtr, retrievedNote.TargetPtr, "Retrieved TargetPtr didn't match value persisted.");
                Assert.AreEqual(note.Text, retrievedNote.Text, "Retrieved NoteText didn't match value persisted.");

                // Test retrieving all notes for a particular user
                IEnumerable<INote> retrievedNotes = Note.GetNotesForUser(userId, 0);

                Assert.AreEqual(1, retrievedNotes.Count(), "Expected exactly one user note to be retrieved for test user from DB.");

                retrievedNote = retrievedNotes.First();

                Assert.AreEqual(note.Id, retrievedNote.Id, "Retrieved NoteId didn't match value persisted.");
                Assert.AreEqual(note.UserId, retrievedNote.UserId, "Retrieved UserId didn't match value persisted.");
                Assert.AreEqual(note.TargetDoc, retrievedNote.TargetDoc, "Retrieved TargetDoc didn't match value persisted.");
                Assert.AreEqual(note.TargetPtr, retrievedNote.TargetPtr, "Retrieved TargetPtr didn't match value persisted.");
                Assert.AreEqual(note.Text, retrievedNote.Text, "Retrieved NoteText didn't match value persisted.");

                // Test deletion of note
                Note.DeleteNoteById(note.Id);
                Assert.IsNull(Note.GetNoteById(oldId), "Note should have been deleted from the database and null returned.");
            }
            finally
            {
                DeleteC2bWebServiceUsers(c2bUser);
                DeleteDestroyerUsers(userId);
            }
        }
    }
}
