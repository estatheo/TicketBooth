An omnichannel ticketing booth w/ twilio and sendgrid 💕

Setup: 

1. create a new sendgrid account @ https://sendgrid.com/
2. create a new twilio account @ https://twlio.com/
3. create a new azure account @ https://portal.azure.com/

Sendgrid setup:

0. setup the mx record on your domain as mx 10 mx.sendgrid.net
1. create an Inbound parse rule @ https://app.sendgrid.com/settings/parse pointing to your azure function (to test it locally, i use ngrok)

Twilio setup:

1. request access to twilio whatsapp beta program
2. (optional) setup a new number
3. (optional) request a new sender for whatsapp 
4. (super optional) request verification for the sender for whatsapp
5. (optional) create a template for your sender (this is gonna be the message used to request user initiation)
6. connect to the sandbox whatsapp on your mobile phone


Azure setup:

1. create a resource group
2. create a storage account


C# Project:

1. Create a new dotnet core project (I decided to use Azure functions with an http trigger)
2. add the following values to your app.settings

    "storageAccount": "AZURE STORAGE ACCOUNT CONNECTION STRING",
    "containerName": "AZURE CONTAINER NAME",
    "accountSid": "TWILIO ACCOUNT SID",
    "accountSecret": "TWILIO ACCOUNT SECRET",
    "fromNumber": "TWILIO SENDER NUMBER OR WHATSAPP SANDBOX NUMBER"

3. run the application locally or deploy it to azure
4. send an email to the domain you registered in the Inbound parse rule in sendgrid
5. add the destination number in the subject ie. +447843753541
6. write a message in the body email that will be sent via whatsapp
7. attach a pdf or a picture of the ticket to the email (multiple attachments supported, thanks twilio!)



