import React, { Component, StyleSheet } from 'react';
import { Button, TextInput, View, TouchableHighlight, Text } from 'react-native'
import auth from '@react-native-firebase/auth';
import LoginFunctions from './LoginFunctions';


export default class EmailProvider extends Component {
  state = {
    email: '',
    password: '',
    password2: '',
    isEmailValid: true,
    isPasswordValid: true,
    showRegister: false
  };

  constructor(props) {
    super(props)
  }

  render() {
    return (
      <View >
        <TextInput
          label={"Email"}
          keyboardType="email-address"
          placeholder="Email"
          onChangeText={text => {
            this.setState({ email: text });
          }}
          error={this.state.isEmailValid}
        />
        <TextInput
          label={"Password"}
          secureTextEntry
          placeholder="Password"
          secureTextEntry={true}
          error={this.state.isPasswordValid}
          onChangeText={text => {
            this.setState({ password: text });
          }} />
        {this.state.showRegister ?
          <View>
            <Button
              title="Sign-In"
              onPress={() => {
                onEmailSigninButtonPress(this.state.email, this.state.password, this.checkValidEmail);
              }} />
            <TouchableHighlight onPress={() => this.toggleRegister()}>
              <Text>Not registered? Sign up here</Text>
            </TouchableHighlight>
          </View> :
          <View>
            <TextInput
              label={"Confirm Password"}
              secureTextEntry
              placeholder="Confirm Password"
              secureTextEntry={true}
              error={this.state.isPasswordValid}
              onChangeText={text => {
                this.setState({ password2: text });
              }} />
            <Button
              title="Register"
              onPress={() => {
                onEmailRegisterButtonPress(this.state.email, this.state.password,
                  this.state.password2, this.checkValidEmail, this.checkValidPassword)
              }} />
            <TouchableHighlight onPress={() => this.toggleRegister()}>
              <Text>Go Back</Text>
            </TouchableHighlight>
          </View>}
      </View>)
  }

  toggleRegister() {
    this.setState({ showRegister: !this.state.showRegister });
  }

  checkValidEmail = (email) => {
    const expression = /(?!.*\.{2})^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([\t]*\r\n)?[\t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([\t]*\r\n)?[\t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i; let re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    let valid = expression.test(String(email)?.toLowerCase());
    if (!valid) {
      console.log('Email is not valid');
    }
    return valid;
  }

  checkValidPassword = (password, password2) => {
    let valid = true;
    if (password.length < 8) {
      valid = false;
      console.log('Password is too short')
    }
    else if (password != password2) {
      valid = false;
      console.log('Passwords do not match')
    }
    return valid;
  }
}

async function onEmailSigninButtonPress(email, password, checkValidEmail) {

  try {
    if (!checkValidEmail(email)) {
      return;
    }
    let credential = await auth().signInWithEmailAndPassword(email, password);

    return LoginFunctions.signInOrLink(auth.EmailAuthProvider.PROVIDER_ID, credential, email);
  }
  catch (error) {
    console.log('Login details not recognised');
  }
}

async function onEmailRegisterButtonPress(email, password, password2,
  checkValidEmail, checkPasswordValid) {
  try {
    if (!checkValidEmail(email)) {
      return;
    }
    if (!checkPasswordValid(password, password2)) {
      return;
    }

    let credential = auth.EmailAuthProvider.credential(email, password);
    return LoginFunctions.registerOrLink(auth.EmailAuthProvider.PROVIDER_ID, credential, email);
  }
  catch (error) {
    console.log(error.message);
  }

}