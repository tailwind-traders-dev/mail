import { beforeAll, afterAll } from "bun:test";
import {DB} from "../lib/models";
beforeAll(async () => {
  // global setup
  await DB.sync();
});

afterAll(async () => {
  // global teardown
  await DB.close();
});