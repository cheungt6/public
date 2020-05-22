FROM zixia/simple-mail-forwarder:1.1
LABEL maintainer="cheungt6"
ARG SMF_EMAILDOMAIN

# https://www.digitalocean.com/community/tutorials/how-to-install-and-configure-dkim-with-postfix-on-debian-wheezy

RUN apk --no-cache add opendkim supervisor

WORKDIR /etc/opendkim

COPY opendkim.conf .
COPY TrustedHosts .

RUN printf "*.$SMF_EMAILDOMAIN\n" >> TrustedHosts
RUN printf "*@$SMF_EMAILDOMAIN mail._domainkey.$SMF_EMAILDOMAIN\n" >> SigningTable
RUN printf "mail._domainkey.$SMF_EMAILDOMAIN $SMF_EMAILDOMAIN:mail:/etc/opendkim/keys/$SMF_EMAILDOMAIN/mail.private\n" >> KeyTable
RUN mkdir keys    

WORKDIR /etc/default

RUN printf "SOCKET=\"inet:8891@localhost\"" >> opendkim

VOLUME /etc/opendkim/keys

WORKDIR /etc/opendkim/keys
        
RUN mkdir $SMF_EMAILDOMAIN && mkdir /var/run/opendkim

WORKDIR /etc/opendkim/keys/$SMF_EMAILDOMAIN

EXPOSE 25
EXPOSE 110
EXPOSE 995
EXPOSE 993
EXPOSE 143
EXPOSE 465
EXPOSE 587
EXPOSE 4190

WORKDIR /etc/postfix/
RUN printf "milter_protocol = 6\n" >> main.cf && \
    printf "milter_default_action = accept\n" >> main.cf && \
    printf "smtpd_milters = inet:localhost:8891\n" >> main.cf && \
    printf "non_smtpd_milters = inet:localhost:8891\n" >> main.cf

RUN mkdir -p /var/log/supervisor && \
    mkdir /var/log/supervisor/conf.d
    
WORKDIR /etc/supervisor/conf.d
COPY supervisord.conf .
    
#ENTRYPOINT ["/usr/bin/supervisord"]
CMD ["/usr/sbin/opendkim -x /etc/opendkim/opendkim.conf"]
