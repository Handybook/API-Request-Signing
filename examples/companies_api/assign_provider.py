import cryptography
import base64
from cryptography.hazmat.backends import default_backend
from cryptography.hazmat.primitives import serialization
from cryptography.hazmat.primitives import hashes
from cryptography.hazmat.primitives.asymmetric import padding
import time
import json
import requests

# path to the private key
private_key_path = 'private.pem'

# url
booking_id = '1234567890'
url = f'https://svccompaniesapi.services.handy-sandbox.com/api/v1/bookings/{booking_id}/provider'

# request data
partner_id = 'partner_name'
http_method = 'put'
timestamp = str(int(time.time()))
payload = json.dumps({'company_provider_id': '123', 'provider_first_name': 'John', 'provider_last_name': 'Smith'})

# request data as string
string = bytes(("\\n").join([partner_id, url, http_method, timestamp, payload]), encoding = 'utf-8')

# signature
private_key_file = open(private_key_path, "rb")
private_key = serialization.load_pem_private_key(private_key_file.read(), password=None, backend=default_backend())
private_key_file.close()
signature = private_key.sign(string, padding.PKCS1v15(), hashes.SHA256())
encoded_signature = base64.b64encode(signature)

# headers
headers = {
    'Content-Type': 'application/json',
    'HDY-PARTNER-ID': partner_id,
    'HDY-TIMESTAMP': timestamp,
    'HDY-SIGNATURE': encoded_signature
}

# request
response = requests.put(url, headers=headers, data=payload)
print(response.content)
print(response.status_code)
