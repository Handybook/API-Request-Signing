module Authorization
  require 'jwt' # https://github.com/jwt/ruby-jwt#readme

  def jwt_valid?
    decoded_jwt =
      begin
        JWT.decode(
        jwt, 
        public_key, 
        true, 
        algorithm: "RS256", 
        iss: "expected_issuer", 
        verify_iss: true)
      rescue JWT::DecodeError => e
        # handle error
      end
    decoded_jwt.present?
  end

  def jwt
    pattern = /^Bearer /
    # Header name may be different based on your environment -- ex: X_HTTP_AUTHORIZATION
    header = request.headers["HTTP_AUTHORIZATION"]
    header.gsub(pattern, '') if header && header.match(pattern)
  end

  def public_key
    # fetch public_key
  end
end
