const {env} = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:52567';

const PROXY_CONFIG = [
  {
    context: ["/user", "/q", "/works", "/forms", "/copyright", "/proxy", "/hubs"],
    target: target,
    secure: false
  },
  {
    context: '/hubs',
    secure: false,
    target: target,
    ws: true
  }
]

module.exports = PROXY_CONFIG;
