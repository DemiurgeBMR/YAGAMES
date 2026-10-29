const { chromium } = require('playwright');
const path = require('path');
const fs = require('fs');

// Запуск: node docs/mockups/src/render.js (нужен playwright)
const root = path.resolve(__dirname, '..');
const ids = ['gameplay', 'win', 'lose', 'hub', 'shop'];
const names = { gameplay: '1-gameplay', win: '2-win', lose: '3-lose', hub: '4-airport', shop: '5-shop' };

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage({ deviceScaleFactor: 2, viewport: { width: 2900, height: 1100 } });
  const errors = [];
  page.on('pageerror', e => errors.push(e.message));
  page.on('console', m => { if (m.type() === 'error') errors.push(m.text()); });
  await page.goto('file://' + path.join(__dirname, 'mockups.html'));
  await page.evaluate(() => document.fonts.ready);
  await page.waitForTimeout(300);
  for (const id of ids) {
    await page.locator('#' + id).screenshot({ path: path.join(root, names[id] + '.png') });
  }
  // overview collage
  const imgs = ids.map(id => 'data:image/png;base64,' + fs.readFileSync(path.join(root, names[id] + '.png')).toString('base64'));
  const labels = ['Уровень', 'Победа', 'Второй шанс', 'Аэропорт', 'Магазин'];
  const p2 = await browser.newPage({ deviceScaleFactor: 1, viewport: { width: 1900, height: 770 } });
  await p2.setContent(`<html><head><style>
    @font-face{font-family:Rubik;src:url(file://${__dirname}/fonts/rubik-cyr.woff2);unicode-range:U+0400-045F;}
    @font-face{font-family:Rubik;src:url(file://${__dirname}/fonts/rubik-lat.woff2);}
    body{margin:0;background:#16203a;font-family:Rubik,sans-serif;color:#fff}
    .wrap{display:flex;gap:28px;padding:30px 36px 36px}
    figure{margin:0;text-align:center} img{width:340px;border-radius:22px;box-shadow:0 12px 30px rgba(0,0,0,.45)}
    figcaption{margin-top:12px;font-size:22px;font-weight:700}
    h1{margin:28px 36px 0;font-size:34px;font-weight:900}
  </style></head><body><h1>Багаж-мастер — макеты экранов</h1><div class="wrap">${imgs.map((s, i) => `<figure><img src="${s}"><figcaption>${labels[i]}</figcaption></figure>`).join('')}</div></body></html>`);
  await p2.evaluate(() => document.fonts.ready);
  await p2.waitForTimeout(300);
  await p2.screenshot({ path: path.join(root, '0-overview.png'), fullPage: true });
  await browser.close();
  console.log(errors.length ? 'ERRORS:\n' + errors.join('\n') : 'ok');
})();
