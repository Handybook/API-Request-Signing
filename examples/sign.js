const fs = require('fs');
const crypto = require('crypto');

class Sample {
  constructor() {};

  sign(privateKey, message) {
    const signature = crypto.sign('sha256', Buffer.from(message), {
      key: privateKey,
      padding: crypto.constants.RSA_PKCS1_PADDING,
    });

    return signature.toString('base64');
  }

  main() {
    const privateKey = fs.readFileSync('test_private_key.pem', 'utf8', (err, data) => data);
    const payload = JSON.stringify({
      user: {
        first_name: 'John',
        last_name: 'Doe',
        email: 'johndoe@example.com',
        phone_number: '2125555555',
      },
      address: {
        address1: '10 Main Street',
        address2: 'Apt 2',
        city: 'New York',
        state: 'NY',
        zipcode: 10012,
        country: 'US',
      },
      order: {
        partner_order_id: '12345',
        products: [
          {
            sku: 'STSP1001',
            product_info: { product_name: 'Arm chair photo' },
            order_details: {
              quantity: 2,
              delivery_date: '2017-04-17T03:50:02.000Z',
            },
          },
        ],
      }
    });
    const partnerId = 'partner_name';
    const url = 'https://partners.services.handy.com/api/v1/orders';
    const httpMethod = 'post';
    const timestamp = Math.floor(Date.now() / 1000);

    const requestMessage = this.requestMessage(partnerId, url, httpMethod, timestamp, payload);
    const signature = this.sign(privateKey, requestMessage);

    console.log(`Input for signing function: ${requestMessage}`);
    console.log(`Signature: ${signature}`);
    console.log(`Required headers:`);
    console.log(`Content-Type: application/json`);
    console.log(`HDY-PARTNER-ID: ${partnerId}`);
    console.log(`HDY-TIMESTAMP: ${timestamp}`);
    console.log(`HDY-SIGNATURE: ${signature}`);
  }

  requestMessage(partnerId, url, httpMethod, timestamp, payload) {
    return `${partnerId}\n${url}\n${httpMethod}\n${timestamp}\n${payload}`;
  }
};

const sign = new Sample();
sign.main();
