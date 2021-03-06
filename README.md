IBM MobileFirst Platform Foundation
===
## PreemptiveLoginWin8
A sample application demonstrating implementation of Preemptive Login.

### Tutorials
https://mobilefirstplatform.ibmcloud.com/tutorials/en/foundation/8.0/authentication-and-security/user-authentication/

### Usage

1. Use either Maven, MobileFirst CLI or your IDE of choice to [build and deploy the available `ResourceAdapter` and `UserLogin` adapters](https://mobilefirstplatform.ibmcloud.com/tutorials/en/foundation/8.0/adapters/creating-adapters/).

 The UserAuthentication Security Check adapter can be found in https://github.com/MobileFirst-Platform-Developer-Center/SecurityCheckAdapters/tree/release80.

2. From a command-line window, navigate to the project's root folder and run the commands:
 - `mfpdev app register` - to register the application.
 - `mfpdev app push` - to map the `accessRestricted` scope to the `UserLogin` security check.

3. Import the project to Visual Studio and [add the MobileFirst Windows SDK](https://mobilefirstplatform.ibmcloud.com/tutorials/en/foundation/8.0/adding-the-mfpf-sdk/windows-8-10/#adding-the-mobilefirst-native-sdk).

### Supported Levels
IBM MobileFirst Platform Foundation 8.0

### License
Copyright 2016 IBM Corp.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
