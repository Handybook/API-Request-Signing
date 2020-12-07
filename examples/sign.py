import cryptography
import base64
from cryptography.hazmat.backends import default_backend
from cryptography.hazmat.primitives import serialization
from cryptography.hazmat.primitives import hashes
from cryptography.hazmat.primitives.asymmetric import padding
import time
import json

class Sample:
    def __init__(self):
        pass

    def sign(self, private_key, message):
        signature = private_key.sign(message, padding.PKCS1v15(), hashes.SHA256())

        signature_encoded = base64.b64encode(signature)
        return signature_encoded

    def main(self):
        # Load PEM encoded private key.
        with open("test_private_key.pem", "rb") as key_file:
            private_key = serialization.load_pem_private_key(key_file.read(), password=None, backend=default_backend())

        # Generate formatted input from request parameters
        payload = "{\"user\":{\"first_name\":\"John\",\"last_name\":\"Doe\",\"email\":\"johndoe@example.com\",\"phone_number\":\"2125555555\"},\"address\":{\"address1\":\"10 Main Street\",\"address2\":\"Apt 2\",\"city\":\"New York\",\"state\":\"NY\",\"zipcode\":10012,\"country\":\"US\"},\"order\":{\"partner_order_id\":\"12345\",\"products\":[{\"sku\":\"STSP1001\",\"product_info\":{\"product_name\":\"Arm chair photo\"},\"order_details\":{\"quantity\":2,\"delivery_date\":\"2017-04-17T03:50:02.000Z\"}}]}}"
        partner_id = 'partner_name'
        url = 'https://partners.services.handy.com/api/v1/orders'
        http_method = 'post'
        timestamp = str(int(time.time()))

        request_message = self.request_message(partner_id, url, http_method, timestamp, payload)

        signature = self.sign(private_key, request_message)

        print("Input for signing function: {}".format(request_message))
        print("Signature: {}".format(signature))
        print("Required headers:")
        print("Content-Type: application/json")
        print("HDY-PARTNER-ID: {}".format(partner_id))
        print("HDY-TIMESTAMP: {}".format(timestamp))
        print("HDY-SIGNATURE: {}".format(signature))

    def request_message(self, partner_id, url, http_method, timestamp, payload):
        return bytes(("\\n").join([partner_id, url, http_method, timestamp, payload]), encoding = 'utf-8')

s = Sample()
s.main()
