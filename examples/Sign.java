import static java.nio.charset.StandardCharsets.UTF_8;

import java.nio.file.Files;
import java.nio.file.Paths;
import java.security.KeyFactory;
import java.security.PrivateKey;
import java.security.Signature;
import java.security.spec.PKCS8EncodedKeySpec;
import java.util.Base64;

public class Sign {
    
    public static PrivateKey getPrivateKey(String keyPath) throws Exception {
    	byte[] keyBytes = Files.readAllBytes(Paths.get(keyPath));
    	PKCS8EncodedKeySpec spec = new PKCS8EncodedKeySpec(keyBytes);
    	KeyFactory kf = KeyFactory.getInstance("RSA");
    	return kf.generatePrivate(spec);
    }

    public static String sign(PrivateKey privateKey, String requestMessage) throws Exception {
    	Signature privateSignature = Signature.getInstance("SHA256withRSA");
        privateSignature.initSign(privateKey);
        privateSignature.update(requestMessage.getBytes(UTF_8));

        byte[] signature = privateSignature.sign();

        return Base64.getEncoder().encodeToString(signature);
    }
    
    public static String requestMessage(String partnerId, String url, String httpMethod, String timestamp, String payload) {
    	StringBuilder sb = new StringBuilder();
    	String separator = "\\n";
        sb.append(partnerId).append(separator);
        sb.append(url).append(separator); 
        sb.append(httpMethod).append(separator); 
        sb.append(timestamp).append(separator); 
        sb.append(payload); 
        return sb.toString();
    }

    public static void main(String... argv) throws Exception {
    	String payload = "{\"user\":{\"first_name\":\"John\",\"last_name\":\"Doe\",\"email\":\"johndoe@example.com\",\"phone_number\":\"2125555555\"},\"address\":{\"address1\":\"10 Main Street\",\"address2\":\"Apt 2\",\"city\":\"New York\",\"state\":\"NY\",\"zipcode\":10012,\"country\":\"US\"},\"order\":{\"partner_order_id\":\"12345\",\"products\":[{\"sku\":\"STSP1001\",\"product_info\":{\"product_name\":\"Arm chair photo\"},\"order_details\":{\"quantity\":2,\"delivery_date\":\"2017-04-17T03:50:02.000Z\"}}]}}";
    	String partnerId = "partner_name";
    	String url = "https://partners.services.handy.com/api/v1/orders";
    	String httpMethod = "post";
    	String timestamp = "" + System.currentTimeMillis() / 1000;

    	String requestMessage = requestMessage(partnerId, url, httpMethod, timestamp, payload);
    	
        /* You will need to convert your private key to PKCS8 format using following command:
         * openssl pkcs8 -topk8 -inform PEM -outform DER -in private.pem -out private.der -nocrypt
         */
    	PrivateKey privateKey = getPrivateKey("test_private_key.der");
        String signature = sign(privateKey, requestMessage);
        System.out.println("Input for signing function: " + requestMessage);
        System.out.println("Signature: " + signature);
        System.out.println("Required headers:");

        System.out.println("Content-Type: application/json");
        System.out.println("HDY-PARTNER-ID: " + partnerId);
        System.out.println("HDY-TIMESTAMP: " + timestamp);
        System.out.println("HDY-SIGNATURE: " + signature);
    }
}
