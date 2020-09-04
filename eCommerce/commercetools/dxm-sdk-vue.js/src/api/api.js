/* eslint-disable import/prefer-default-export */
import config from '../../sunrise.config';
import { getAuthToken } from '../auth';
import { group } from './lib';

export const withPage = ({
  page = 1,
  pageSize = 20,
  ...query
}) => ({
  page,
  pageSize,
  ...query,
});
const fetchJson = (...args) => fetch(...args).then((result) => {
  if (result.status === 401) {
    // eslint-disable-next-line no-throw-literal
    throw { statusCode: 401 };
  }
  return result.json();
});
export const groupFetchJson = group(fetchJson);

export const baseUrl = `${config.ct.api}/${config.ct.auth.projectKey}`;
export const withToken = (() => {
  let tries = 0;
  return function tryRequest(fn, error) {
    const doRequest = (...args) => getAuthToken(error)
      .then((tk) => fn(...args.concat(tk)))
      .catch((err) => {
        tries += 1;
        if (err.statusCode === 401 && tries < 3) {
          return tryRequest(fn, true)(...args);
        }
        throw err;
      });
    return doRequest;
  };
})();
export const makeConfig = (token) => ({
  headers: {
    accept: '*/*',
    authorization: token,
    'content-type': 'application/json',
    'sec-fetch-mode': 'cors',
    'sec-fetch-site': 'cross-site',
  },
  mode: 'cors',
});

export const toUrl = (
  base,
  query,
) => {
  const url = new URL(base);
  const pageSize = query.find(([key]) => key === 'pageSize')[1];
  const page = query.find(([key]) => key === 'page')[1];
  query
    .filter(
      ([k, v]) => v !== undefined
        && !['pageSize', 'page'].includes(k),
    )
    .concat([['limit', pageSize]])
    .concat([['offset', pageSize * (page - 1)]])
    .reduce(
      (result, [key, value]) => {
        if (Array.isArray(value)) {
          return result.concat(
            value.map(
              (v) => [key, v],
            ),
          );
        }
        return result.concat([[key, value]]);
      }, [],
    )
    .forEach(
      ([key, val]) => url.searchParams.append(key, val),
    );
  return url.toString();
};
