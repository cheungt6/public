import React, { Component } from 'react';
import { Button } from 'react-native'
import auth from '@react-native-firebase/auth';
import { GoogleSignin } from '@react-native-community/google-signin';
import LoginFunctions from './LoginFunctions';

const webClientId =  "YOUR_KEY_HERE";

export default class GoogleProvider extends Component {
  constructor(props) {
    super(props)
  }

  render() {
    return (
      <Button
        title="Google Sign-In"
        onPress={() => {
          onGoogleButtonPress();
        }}
      />
    );
  }
};

async function onGoogleButtonPress() {
  try {
    GoogleSignin.configure({
      webClientId: this.webClientId,
      scopes: ['email', 'profile']
    })

    const data = await GoogleSignin.signIn();
    let credential = auth.GoogleAuthProvider.credential(data.idToken);

    return LoginFunctions.signInOrLink(auth.GoogleAuthProvider.PROVIDER_ID, credential, data.user.email);
  }
  catch (error) {
    console.log(error.message);
  }
}