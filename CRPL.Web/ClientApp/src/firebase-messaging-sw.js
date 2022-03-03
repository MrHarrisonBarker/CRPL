// Give the service worker access to Firebase Messaging.
// Note that you can only use Firebase Messaging here, other Firebase libraries
// are not available in the service worker.
self.importScripts('https://www.gstatic.com/firebasejs/9.4.0/firebase-app-compat.js');
self.importScripts('https://www.gstatic.com/firebasejs/9.4.0/firebase-messaging-compat.js');

// Initialize the Firebase app in the service worker by passing in the
// messagingSenderId.

firebase.initializeApp({
  apiKey: 'AIzaSyCvBytHcZHoHlC-vCi8XsmkpSz0uDfn7GA',
  authDomain: 'crpl-c5132.firebaseapp.com',
  databaseURL: 'https://crpl-c5132-default-rtdb.europe-west1.firebasedatabase.app/',
  projectId: 'crpl-c5132',
  storageBucket: 'crpl-c5132.appspot.com',
  messagingSenderId: '30934328073',
  appId: "1:30934328073:web:6c179d23df67985e60a9d8"
});

// Retrieve an instance of Firebase Messaging so that it can handle background
// messages.
const messaging = firebase.messaging();
