import auth from '@react-native-firebase/auth';
import { AsyncStorage } from '@react-native-community/async-storage';

const LoginFunctions = {

  signInOrLink: async function (provider, credential, email) {
    this.saveCredential(provider, credential)
    await auth().signInWithCredential(credential).catch(
      async (error) => {
        try {
          if (error.code != "auth/account-exists-with-different-credential") {
            throw error;
          }
          let methods = await auth().fetchSignInMethodsForEmail(email);
          let oldCred = await this.getCredential(methods[0]);
          let prevUser = await auth().signInWithCredential(oldCred);
          auth().currentUser.linkWithCredential(credential);
        }
        catch (error) {
          throw error;
        }
      }
    );
  },

  registerOrLink: async function (provider, credential, email) {
    this.saveCredential(provider, credential)
    let user = await auth().createUserWithEmailAndPassword(credential.token, credential.secret).catch(
      async (error) => {
        try {
          if (error.code != "auth/email-already-in-use") {
            let a = 1;
            throw error;
          }
          let methods = await auth().fetchSignInMethodsForEmail(email);
          let oldCred = await this.getCredential(methods[0]);
          let prevUser = await auth().signInWithCredential(oldCred);
          auth().currentUser.linkWithCredential(credential);
        }
        catch (error) {
          throw error;
        }
      }
    );
    await user.sendEmailVerification();
  },

  getCredential: async function (provider) {
    try {
      let value = await AsyncStorage.getItem(provider);
      if (value !== null) {
        let [token, secret] = JSON.parse(value);
        return this.getProvider(provider).credential(token, secret);
      }
    } catch (error) {
      throw error;
    }
  },

  saveCredential: async function (provider, credential) {
    try {
      let saveData = JSON.stringify([credential.token, credential.secret])
      await AsyncStorage.setItem(
        provider,
        saveData
      );
    } catch (error) {
      throw error;
    }

  },

  getProvider: function (providerId) {
    switch (providerId) {
      case auth.GoogleAuthProvider.PROVIDER_ID:
        return auth.GoogleAuthProvider;
      case auth.FacebookAuthProvider.PROVIDER_ID:
        return auth.FacebookAuthProvider;
      case auth.TwitterAuthProvider.PROVIDER_ID:
        return auth.TwitterAuthProvider;
      case auth.EmailAuthProvider.PROVIDER_ID:
        return auth.EmailAuthProvider;
      default:
        throw new Error(`No provider implemented for ${providerId}`);
    }
  },
}
export default LoginFunctions;