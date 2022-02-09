const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:52567';

const PROXY_CONFIG = [
  {
    context: ["/user", "/user/search"],
    target: target,
    secure: false
  },
  {
    context: ["/works",],
    target: target,
    secure: false
  },
  {
    context: ["/forms",],
    target: target,
    secure: false
  },
  {
    context: ["/copyright",],
    target: target,
    secure: false
  }
]

module.exports = PROXY_CONFIG;
