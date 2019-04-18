<?php
class Signer {
  private $key_id;
  private $signature_algorithm;
  public function __construct($private_key) {
    $this->key_id = openssl_get_privatekey($private_key);
    $this->signature_algorithm = "SHA256";
  }
  public function getSignature ($partner_id, $url, $http_method, $timestamp, $request_body) {
    $data = $partner_id . '\n' . $url . '\n' . strtolower($http_method) . '\n' . $timestamp . '\n' . $request_body;
    openssl_sign($data, $signature, $this->key_id, $this->signature_algorithm);
    return base64_encode($signature);
  }

