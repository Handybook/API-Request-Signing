# Handy Request Signing
In order to communicate with Handy's partner API you must generate a signature for each request sent using the RSA-SHA256 algorithm. The signature and other metadata such as your partner ID, and timestamp are sent along in HTTP headers with the request. 

Generating a unique signature for each request based on the http request, payload, and timestamp ensures the integrity and authenticity of the information being sent to our API.  The security benefits of this are:

- Using a public/private key pair means that only you have access to your private key and not even Handy can impersonate you to our API.
- Signed timestamps prevent replay attacks.
- Signed payloads prevent request tampering.

In order to begin, you must first generate a public/private RSA keypair and send us your public key.

## Generating a Keypair
(NOTE: You only have to do this step once.)

There are many ways to generate an RSA keypair. This easiest way is probably using the `openssl` tool. You can install it like this:

- Windows:
	- https://wiki.openssl.org/index.php/Binaries
- Linux:
	- `sudo apt-get install openssl`
- OSX:
	- `brew install openssl`

Bash:
```bash
openssl genrsa -out private.pem 2048
openssl rsa -pubout -in private.pem -out public.pem
```
After running these two commands, you should now have two files. 

`private.pem`  - This is your private key. Keep this safe and don't give it to anyone. This is what you will use to sign requests.
`public.pem` - This is your public key. It will be used to verify your signed requests. This is what you will give to us so we can authenticate you on our API.


## How to sign requests

In order to generate a signature you need the following information:

- Your `PARTNER_ID` which will be agreed upon in advance.
- The `URL` of the request being made.
- The `HTTP_METHOD` of the request being made.
- The `PAYLOAD` or body of the request being made.
- The current `TIMESTAMP` represented as an integer of seconds since epoch UTC. 

Your signature generation function will look something like this:
```
message = PARTNER_ID + "\n" + URL + "\n" + HTTP_METHOD + "\n" + TIMESTAMP + "\n" + PAYLOAD
padding = PKCS1v15
algorithm = SHA256
base64(private_key.sign(message, padding, algorithm))
```
- `base64` is Strict Base64 encoding (RFC 4648). No line feeds are added.
- The hashing algorithm is SHA256 using your RSA private key.
- Padding is PKCS1v15 if you need to specify.

Please check out our examples to see this done in various languages.

## How to send the request

You should now have built a function that takes the above parameters and outputs a signature. You will include that signature in the request along with your partner_id and timestamp using custom headers. (HDY-PARTNER-ID, HDY-TIMESTAMP, and HDY-SIGNATURE)

Example request:
```
POST https://partners.services.handy.com/api/v1/orders
Host: partners.services.handy.com
Accept: application/json
HDY-PARTNER-ID: (Your Partner ID)
HDY-TIMESTAMP: 1525361611
HDY-SIGNATURE: Gkdh9rty9740RIeZtkIDs2RqvXrP7eq6k2/QkpxmJH1wKmSvfmzPr4jRR...

{
  "user": {
    "id": "xzvdfhryhbdbe",
    "email": "example@handy.com",
    ...
  },
  "address": {
    "id": "sfgethrethrt",
    "address1": "1st street",
    ...
  },
  "order": {
    "partner_order_id": "110001023",
    ...
  }
}
```




