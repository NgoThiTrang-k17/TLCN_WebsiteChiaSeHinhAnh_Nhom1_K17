import React from "react";
import LottieView from "lottie-react-native";

import { Spacer } from "../../../components/spacer/spacer.component";
import {
  AccountBackground,
  AccountContainerHome,
  AccountCover,
  AuthButton,
  Title,
  AnimationWrapper,
} from "../components/account.styles";
import { fontSizes, fontWeights } from "../../../infrastructure/theme/fonts";

export const AccountScreen = ({ navigation }) => {
  return (
    <AccountBackground>     
      <AccountContainerHome>
        <AccountCover />
        <AnimationWrapper>
            <LottieView
            key="animation"
            autoPlay
            loop
            resizeMode="cover"
            source={require("../../../../assets/12268-photo.json")}
            />
        </AnimationWrapper>
        <Title>IS</Title>
        <AuthButton
          icon="lock-open-outline"
          mode="contained"
          onPress={() => navigation.navigate("Login")}
        >
          Login
        </AuthButton>
        <Spacer size="large">
          <AuthButton
            icon="email"
            mode="contained"
            onPress={() => navigation.navigate("Register")}
          >
            Register
          </AuthButton>
        </Spacer>
      </AccountContainerHome>
    </AccountBackground>
  );
};
