import React, { Component } from 'react';
import { Button } from 'react-native';
import auth from '@react-native-firebase/auth';
import { NativeModules } from 'react-native';
import LoginFunctions from './LoginFunctions';

const { RNTwitterSignIn } = NativeModules;
const config = {
  consumer_key: "YOUR_KEY_HERE",
  consumer_secret: "YOUR_SECRET_HERE"
};
export default class TwitterProvider extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <Button
        title="Twitter Sign-In"
        onPress={() => { onTwitterButtonPress(); }}
      />
    );
  }
}

async function onTwitterButtonPress() {
  try {
    RNTwitterSignIn.init(config.consumer_key, config.consumer_secret)
    var loginData = await RNTwitterSignIn.logIn();

    const { authToken, authTokenSecret, email } = loginData;
    const credential = auth.TwitterAuthProvider.credential(authToken, authTokenSecret)
    await LoginFunctions.signInOrLink(auth.TwitterAuthProvider.PROVIDER_ID, credential, email)
  }
  catch (err) {
    console.log(error.message);
  }
}