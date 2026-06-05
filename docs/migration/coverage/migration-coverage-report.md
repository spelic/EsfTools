# ESF migration coverage report

- Input: `C:\esf-latest`
- Programs analyzed: **1127**
- Generation: off · Build: off

## 1. Executive Summary

- Parse failed: 0 · Model failed: 0 · Generation failed: 0
- Functions skipped during generation (total): 0
- Avg unknown-statement %: 0,03

| Risk level | Programs |
|---|---:|
| Low | 11 |
| Medium | 35 |
| High | 430 |
| Blocked | 651 |

## 2. Top Pilot Candidates

| Program | Risk | Level | Action | Fns | Stmts | Unk% | Build |
|---|---:|---|---|---:|---:|---:|---|
| CALL_NAZIV_IZDELKA | 0 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| IMP01_V11 | 0 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| LINKAGE | 0 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| S617_V04 | 0 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| SE31M04 | 0 | Low | Pilot candidate (verify with build) | 7 | 84 | 0 | NotAttempted |
| TACEL | 0 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| UU10A | 0 | Low | Pilot candidate (verify with build) | 8 | 10 | 0 | NotAttempted |
| D315A_V01 | 15 | Low | Pilot candidate (verify with build) | 10 | 121 | 0 | NotAttempted |
| D336A_V03 | 15 | Low | Pilot candidate (verify with build) | 7 | 117 | 0 | NotAttempted |
| SPGBA_V02 | 15 | Low | Pilot candidate (verify with build) | 32 | 423 | 0 | NotAttempted |
| TE99W01 | 15 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |

## 3. Highest Risk Programs

| Program | Risk | Level | Action | Fns | Stmts | Unk% | Build |
|---|---:|---|---|---:|---:|---:|---|
| CE04A_V32 | 100 | Blocked | Extend EZE runtime | 93 | 2100 | 0 | NotAttempted |
| CT11A_V06 | 100 | Blocked | Extend EZE runtime | 25 | 505 | 0 | NotAttempted |
| CT26A_V34 | 100 | Blocked | Extend EZE runtime | 84 | 2144 | 0 | NotAttempted |
| D002A_V11 | 100 | Blocked | Extend EZE runtime | 31 | 653 | 0 | NotAttempted |
| D006A_V49 | 100 | Blocked | Extend EZE runtime | 79 | 2125 | 0 | NotAttempted |
| D012A_V06 | 100 | Blocked | Extend EZE runtime | 14 | 513 | 0 | NotAttempted |
| D103A_V24 | 100 | Blocked | Extend EZE runtime | 43 | 1053 | 0 | NotAttempted |
| D107A_V42 | 100 | Blocked | Extend EZE runtime | 85 | 2464 | 0 | NotAttempted |
| D110A_V89 | 100 | Blocked | Extend EZE runtime | 172 | 5021 | 0 | NotAttempted |
| D113A_V31 | 100 | Blocked | Extend EZE runtime | 53 | 1550 | 0 | NotAttempted |
| D116A_V85 | 100 | Blocked | Extend EZE runtime | 142 | 5008 | 0 | NotAttempted |
| D117A_V97 | 100 | Blocked | Extend EZE runtime | 132 | 4496 | 0 | NotAttempted |
| D118A_V62 | 100 | Blocked | Extend EZE runtime | 93 | 1972 | 0 | NotAttempted |
| D119A_V21 | 100 | Blocked | Extend EZE runtime | 44 | 1087 | 0 | NotAttempted |
| D120A_V22 | 100 | Blocked | Extend EZE runtime | 39 | 994 | 0 | NotAttempted |
| D121A_V75 | 100 | Blocked | Extend EZE runtime | 145 | 4219 | 0 | NotAttempted |
| D125A_V73 | 100 | Blocked | Extend EZE runtime | 228 | 8845 | 0 | NotAttempted |
| D127A_V45 | 100 | Blocked | Extend EZE runtime | 93 | 2953 | 0 | NotAttempted |
| D129A_V93 | 100 | Blocked | Extend EZE runtime | 152 | 4795 | 0 | NotAttempted |
| D131A_V98 | 100 | Blocked | Extend EZE runtime | 127 | 5327 | 0 | NotAttempted |
| D134A_V28 | 100 | Blocked | Extend EZE runtime | 53 | 1754 | 0 | NotAttempted |
| D135A_V45 | 100 | Blocked | Extend EZE runtime | 104 | 2854 | 0 | NotAttempted |
| D136A_V06 | 100 | Blocked | Extend EZE runtime | 229 | 6140 | 0 | NotAttempted |
| D137A_V80 | 100 | Blocked | Extend EZE runtime | 253 | 7957 | 0 | NotAttempted |
| D139A_V37 | 100 | Blocked | Extend EZE runtime | 92 | 2684 | 0 | NotAttempted |

## 4. Parse / Model / Generation / Build Status Summary

| Stage | OK | Failed |
|---|---:|---:|
| Parse | 1127 | 0 |
| Model build | 1127 | 0 |

## 5. Unsupported Statement Summary

| Unknown statement text | Programs |
|---|---:|
| EZESBLKT(TARGET); | 14 |
| EZESCOPY( TARGET,TAROD,TARDOL,SOURCE,SOUROD,SOURDOL); | 14 |
| EZESNULT(TARGET); | 14 |
| EZESSET( TARGET,TAROD,TARDOL,SEARCH); | 14 |
| EZESNULT(PZ04M01.STARANJEVAR); | 1 |

## 6. Unsupported EZE Word Summary

| EZE word | Programs |
|---|---:|
| EZEMSG | 774 |
| EZEDESTP | 291 |
| EZECNVCM | 191 |
| EZESCCWS | 69 |
| EZESQRT | 24 |
| EZEABS | 21 |
| EZESFIND | 21 |
| EZEPOW | 19 |
| EZEEXP | 16 |
| EZELOG | 15 |
| EZESNULT | 15 |
| EZEACOS | 14 |
| EZEASIN | 14 |
| EZEATAN | 14 |
| EZEATAN2 | 14 |
| EZECEIL | 14 |
| EZECOS | 14 |
| EZECOSH | 14 |
| EZEFLADD | 14 |
| EZEFLDIV | 14 |
| EZEFLMOD | 14 |
| EZEFLMUL | 14 |
| EZEFLOOR | 14 |
| EZEFLSET | 14 |
| EZEFLSUB | 14 |
| _…and 30 more_ | |

## 7. Unsupported SQL Option Summary

| SQL option | Programs |
|---|---:|
| DISPLAY | 238 |

## 8. Feature Usage Summary

| Feature | Programs |
|---|---:|
| CONVERSE | 1070 |
| DXFR | 992 |
| XFER | 1 |
| DISPLAY | 182 |
| CALL | 1118 |
| MOVEA | 227 |
| SQL | 959 |
| SQL cursor flow | 723 |
| Packed/zoned decimals | 1119 |
| OCCURS arrays | 1102 |
| Redefined records | 2 |
| Map edit routines | 3 |

## 9. Full Program Table

| Program | Risk | Level | Action | Fns | Stmts | Unk% | Build |
|---|---:|---|---|---:|---:|---:|---|
| CE04A_V32 | 100 | Blocked | Extend EZE runtime | 93 | 2100 | 0 | NotAttempted |
| CT11A_V06 | 100 | Blocked | Extend EZE runtime | 25 | 505 | 0 | NotAttempted |
| CT26A_V34 | 100 | Blocked | Extend EZE runtime | 84 | 2144 | 0 | NotAttempted |
| D002A_V11 | 100 | Blocked | Extend EZE runtime | 31 | 653 | 0 | NotAttempted |
| D006A_V49 | 100 | Blocked | Extend EZE runtime | 79 | 2125 | 0 | NotAttempted |
| D012A_V06 | 100 | Blocked | Extend EZE runtime | 14 | 513 | 0 | NotAttempted |
| D103A_V24 | 100 | Blocked | Extend EZE runtime | 43 | 1053 | 0 | NotAttempted |
| D107A_V42 | 100 | Blocked | Extend EZE runtime | 85 | 2464 | 0 | NotAttempted |
| D110A_V89 | 100 | Blocked | Extend EZE runtime | 172 | 5021 | 0 | NotAttempted |
| D113A_V31 | 100 | Blocked | Extend EZE runtime | 53 | 1550 | 0 | NotAttempted |
| D116A_V85 | 100 | Blocked | Extend EZE runtime | 142 | 5008 | 0 | NotAttempted |
| D117A_V97 | 100 | Blocked | Extend EZE runtime | 132 | 4496 | 0 | NotAttempted |
| D118A_V62 | 100 | Blocked | Extend EZE runtime | 93 | 1972 | 0 | NotAttempted |
| D119A_V21 | 100 | Blocked | Extend EZE runtime | 44 | 1087 | 0 | NotAttempted |
| D120A_V22 | 100 | Blocked | Extend EZE runtime | 39 | 994 | 0 | NotAttempted |
| D121A_V75 | 100 | Blocked | Extend EZE runtime | 145 | 4219 | 0 | NotAttempted |
| D125A_V73 | 100 | Blocked | Extend EZE runtime | 228 | 8845 | 0 | NotAttempted |
| D127A_V45 | 100 | Blocked | Extend EZE runtime | 93 | 2953 | 0 | NotAttempted |
| D129A_V93 | 100 | Blocked | Extend EZE runtime | 152 | 4795 | 0 | NotAttempted |
| D131A_V98 | 100 | Blocked | Extend EZE runtime | 127 | 5327 | 0 | NotAttempted |
| D134A_V28 | 100 | Blocked | Extend EZE runtime | 53 | 1754 | 0 | NotAttempted |
| D135A_V45 | 100 | Blocked | Extend EZE runtime | 104 | 2854 | 0 | NotAttempted |
| D136A_V06 | 100 | Blocked | Extend EZE runtime | 229 | 6140 | 0 | NotAttempted |
| D137A_V80 | 100 | Blocked | Extend EZE runtime | 253 | 7957 | 0 | NotAttempted |
| D139A_V37 | 100 | Blocked | Extend EZE runtime | 92 | 2684 | 0 | NotAttempted |
| D142A_V38 | 100 | Blocked | Extend EZE runtime | 170 | 7456 | 0 | NotAttempted |
| D146A_V04 | 100 | Blocked | Extend EZE runtime | 35 | 641 | 0 | NotAttempted |
| D147A_V03 | 100 | Blocked | Extend EZE runtime | 13 | 428 | 0 | NotAttempted |
| D148A_V03 | 100 | Blocked | Extend EZE runtime | 15 | 440 | 0 | NotAttempted |
| D150A_V35 | 100 | Blocked | Extend EZE runtime | 105 | 3108 | 0 | NotAttempted |
| D154A_V16 | 100 | Blocked | Extend EZE runtime | 34 | 1179 | 0 | NotAttempted |
| D155A_V03 | 100 | Blocked | Extend EZE runtime | 55 | 1237 | 0 | NotAttempted |
| D156A_V08 | 100 | Blocked | Extend EZE runtime | 197 | 6144 | 0 | NotAttempted |
| D157A_V99 | 100 | Blocked | Extend EZE runtime | 131 | 4962 | 0 | NotAttempted |
| D159A_V14 | 100 | Blocked | Extend EZE runtime | 28 | 556 | 0 | NotAttempted |
| D164A_V12 | 100 | Blocked | Extend EZE runtime | 58 | 1907 | 0 | NotAttempted |
| D165A_V05 | 100 | Blocked | Extend EZE runtime | 15 | 412 | 0 | NotAttempted |
| D166A_V15 | 100 | Blocked | Extend EZE runtime | 39 | 990 | 0 | NotAttempted |
| D168A_V17 | 100 | Blocked | Extend EZE runtime | 31 | 1551 | 0 | NotAttempted |
| D170A_V15 | 100 | Blocked | Extend EZE runtime | 52 | 1822 | 0 | NotAttempted |
| D171A_V03 | 100 | Blocked | Extend EZE runtime | 48 | 1294 | 0 | NotAttempted |
| D172A_V05 | 100 | Blocked | Extend EZE runtime | 31 | 986 | 0 | NotAttempted |
| D174A_V07 | 100 | Blocked | Extend EZE runtime | 20 | 492 | 0 | NotAttempted |
| D175A_V09 | 100 | Blocked | Extend EZE runtime | 62 | 1047 | 0 | NotAttempted |
| D176A_V11 | 100 | Blocked | Extend EZE runtime | 63 | 1200 | 0 | NotAttempted |
| D177A_V30 | 100 | Blocked | Extend EZE runtime | 71 | 2178 | 0 | NotAttempted |
| D180A_V06 | 100 | Blocked | Extend EZE runtime | 36 | 672 | 0 | NotAttempted |
| D190A_V06 | 100 | Blocked | Extend EZE runtime | 26 | 486 | 0 | NotAttempted |
| D191A_V09 | 100 | Blocked | Extend EZE runtime | 33 | 876 | 0 | NotAttempted |
| D192A_V17 | 100 | Blocked | Extend EZE runtime | 36 | 778 | 0 | NotAttempted |
| D193A_V61 | 100 | Blocked | Extend EZE runtime | 83 | 2428 | 0 | NotAttempted |
| D202A_V11 | 100 | Blocked | Extend EZE runtime | 26 | 643 | 0 | NotAttempted |
| D203A_V12 | 100 | Blocked | Extend EZE runtime | 261 | 8527 | 0 | NotAttempted |
| D205A_V02 | 100 | Blocked | Extend EZE runtime | 59 | 1763 | 0 | NotAttempted |
| D206A_V79 | 100 | Blocked | Extend EZE runtime | 172 | 4456 | 0 | NotAttempted |
| D211A_V03 | 100 | Blocked | Extend EZE runtime | 29 | 958 | 0 | NotAttempted |
| D216A_V04 | 100 | Blocked | Extend EZE runtime | 102 | 2909 | 0 | NotAttempted |
| D232A_V58 | 100 | Blocked | Extend EZE runtime | 62 | 2768 | 0 | NotAttempted |
| D234A_V08 | 100 | Blocked | Extend EZE runtime | 34 | 940 | 0 | NotAttempted |
| D237A_V24 | 100 | Blocked | Extend EZE runtime | 65 | 1785 | 0 | NotAttempted |
| D240A_V04 | 100 | Blocked | Extend EZE runtime | 38 | 862 | 0 | NotAttempted |
| D247A_V08 | 100 | Blocked | Extend EZE runtime | 31 | 740 | 0 | NotAttempted |
| D248A_V07 | 100 | Blocked | Extend EZE runtime | 79 | 1809 | 0 | NotAttempted |
| D249A_V07 | 100 | Blocked | Extend EZE runtime | 46 | 1255 | 0 | NotAttempted |
| D250A_V10 | 100 | Blocked | Extend EZE runtime | 43 | 1130 | 0 | NotAttempted |
| D252A_V015 | 100 | Blocked | Extend EZE runtime | 28 | 688 | 0 | NotAttempted |
| D253A_V02 | 100 | Blocked | Extend EZE runtime | 28 | 855 | 0 | NotAttempted |
| D254A_V04 | 100 | Blocked | Extend EZE runtime | 27 | 664 | 0 | NotAttempted |
| D256A_V02 | 100 | Blocked | Extend EZE runtime | 57 | 891 | 0 | NotAttempted |
| D264A_V04 | 100 | Blocked | Extend EZE runtime | 25 | 500 | 0 | NotAttempted |
| D273A_V32 | 100 | Blocked | Extend EZE runtime | 151 | 3318 | 0 | NotAttempted |
| D274A_V06 | 100 | Blocked | Extend EZE runtime | 22 | 445 | 0 | NotAttempted |
| D275A_V02 | 100 | Blocked | Extend EZE runtime | 23 | 436 | 0 | NotAttempted |
| D286A_V02 | 100 | Blocked | Extend EZE runtime | 14 | 224 | 0 | NotAttempted |
| D288A_V01 | 100 | Blocked | Extend EZE runtime | 26 | 571 | 0 | NotAttempted |
| D298A_V01 | 100 | Blocked | Extend EZE runtime | 38 | 700 | 0 | NotAttempted |
| D300A_V02 | 100 | Blocked | Extend EZE runtime | 41 | 839 | 0 | NotAttempted |
| D302A_V01 | 100 | Blocked | Extend EZE runtime | 15 | 286 | 0 | NotAttempted |
| D304A_V01 | 100 | Blocked | Extend EZE runtime | 27 | 558 | 0 | NotAttempted |
| D312A_V32 | 100 | Blocked | Extend EZE runtime | 82 | 1927 | 0 | NotAttempted |
| D313A_V05 | 100 | Blocked | Extend EZE runtime | 18 | 348 | 0 | NotAttempted |
| D325A_V06 | 100 | Blocked | Extend EZE runtime | 34 | 656 | 0 | NotAttempted |
| DL12A_V31 | 100 | Blocked | Extend EZE runtime | 68 | 2160 | 0 | NotAttempted |
| DL13A_V40 | 100 | Blocked | Extend EZE runtime | 57 | 1644 | 0 | NotAttempted |
| DL26A_V53 | 100 | Blocked | Extend EZE runtime | 127 | 4156 | 0 | NotAttempted |
| DL27A_V31 | 100 | Blocked | Extend EZE runtime | 39 | 1063 | 0 | NotAttempted |
| DL34A_V27 | 100 | Blocked | Extend EZE runtime | 33 | 887 | 0 | NotAttempted |
| DL35A_V12 | 100 | Blocked | Extend EZE runtime | 24 | 517 | 0 | NotAttempted |
| DL92A_V20 | 100 | Blocked | Extend EZE runtime | 29 | 786 | 0 | NotAttempted |
| DL93A_V32 | 100 | Blocked | Extend EZE runtime | 62 | 1409 | 0 | NotAttempted |
| DL94A_V12 | 100 | Blocked | Extend EZE runtime | 23 | 582 | 0 | NotAttempted |
| F055A_V10NP | 100 | Blocked | Extend EZE runtime | 83 | 1952 | 0 | NotAttempted |
| F211A_V12 | 100 | Blocked | Extend EZE runtime | 58 | 2241 | 0 | NotAttempted |
| F253A_V20 | 100 | Blocked | Extend EZE runtime | 62 | 1078 | 0 | NotAttempted |
| F282A_V09 | 100 | Blocked | Extend EZE runtime | 24 | 408 | 0 | NotAttempted |
| F283A_V19 | 100 | Blocked | Extend EZE runtime | 27 | 577 | 0 | NotAttempted |
| F340A_V04 | 100 | Blocked | Extend EZE runtime | 15 | 336 | 0 | NotAttempted |
| IN82A_V45 | 100 | Blocked | Extend EZE runtime | 56 | 1146 | 0 | NotAttempted |
| IZ10A_V13 | 100 | Blocked | Extend EZE runtime | 39 | 1047 | 0 | NotAttempted |
| IZ20A_V95 | 100 | Blocked | Extend EZE runtime | 136 | 3589 | 0 | NotAttempted |
| IZ25A_V14 | 100 | Blocked | Extend EZE runtime | 34 | 789 | 0 | NotAttempted |
| IZ26A_V06 | 100 | Blocked | Extend EZE runtime | 18 | 342 | 0 | NotAttempted |
| IZ27A_V05 | 100 | Blocked | Extend EZE runtime | 15 | 245 | 0 | NotAttempted |
| IZ28A_V12 | 100 | Blocked | Extend EZE runtime | 30 | 717 | 0 | NotAttempted |
| IZ29A_V06 | 100 | Blocked | Extend EZE runtime | 16 | 282 | 0 | NotAttempted |
| IZ30A_V09 | 100 | Blocked | Extend EZE runtime | 21 | 402 | 0 | NotAttempted |
| IZ31A_V15 | 100 | Blocked | Extend EZE runtime | 65 | 1582 | 0 | NotAttempted |
| IZ32A_V07 | 100 | Blocked | Extend EZE runtime | 21 | 831 | 0 | NotAttempted |
| IZ33A_V07 | 100 | Blocked | Extend EZE runtime | 25 | 425 | 0 | NotAttempted |
| KA11A_V06 | 100 | Blocked | Extend EZE runtime | 15 | 220 | 0 | NotAttempted |
| KA12A_V09 | 100 | Blocked | Extend EZE runtime | 28 | 467 | 0 | NotAttempted |
| KA20A_V28 | 100 | Blocked | Extend EZE runtime | 31 | 1152 | 0 | NotAttempted |
| KA44A_V05 | 100 | Blocked | Extend EZE runtime | 31 | 1132 | 0 | NotAttempted |
| KA50A_V14 | 100 | Blocked | Extend EZE runtime | 38 | 1769 | 0 | NotAttempted |
| KA50A_V15_EUR | 100 | Blocked | Extend EZE runtime | 38 | 1768 | 0 | NotAttempted |
| KA60A_V08 | 100 | Blocked | Extend EZE runtime | 37 | 1622 | 0 | NotAttempted |
| LS10A_V04 | 100 | Blocked | Extend EZE runtime | 43 | 1550 | 0 | NotAttempted |
| LS21A_V11 | 100 | Blocked | Extend EZE runtime | 79 | 1316 | 0 | NotAttempted |
| MF55A_V54 | 100 | Blocked | Extend EZE runtime | 69 | 1914 | 0 | NotAttempted |
| MF76A_V07 | 100 | Blocked | Extend EZE runtime | 29 | 403 | 0 | NotAttempted |
| MF86A_V05 | 100 | Blocked | Extend EZE runtime | 33 | 582 | 0 | NotAttempted |
| MF87A_V09 | 100 | Blocked | Extend EZE runtime | 39 | 680 | 0 | NotAttempted |
| MI31A_V32 | 100 | Blocked | Extend EZE runtime | 85 | 2090 | 0 | NotAttempted |
| NA37A_V24 | 100 | Blocked | Extend EZE runtime | 32 | 819 | 0 | NotAttempted |
| NA55A_V16A | 100 | Blocked | Extend EZE runtime | 74 | 2903 | 0 | NotAttempted |
| NA55A_V21 | 100 | Blocked | Extend EZE runtime | 74 | 2940 | 0 | NotAttempted |
| NA58A_V31 | 100 | Blocked | Extend EZE runtime | 56 | 1394 | 0 | NotAttempted |
| NA70A_V27 | 100 | Blocked | Extend EZE runtime | 138 | 3406 | 0 | NotAttempted |
| NA71A_V11 | 100 | Blocked | Extend EZE runtime | 43 | 953 | 0 | NotAttempted |
| NA73A_V09 | 100 | Blocked | Extend EZE runtime | 35 | 1063 | 0 | NotAttempted |
| NA74A_V10 | 100 | Blocked | Extend EZE runtime | 37 | 920 | 0 | NotAttempted |
| NA82A_V05 | 100 | Blocked | Extend EZE runtime | 34 | 1127 | 0 | NotAttempted |
| PO10A_V25 | 100 | Blocked | Extend EZE runtime | 27 | 866 | 0 | NotAttempted |
| PR10A_V05 | 100 | Blocked | Extend EZE runtime | 14 | 315 | 0 | NotAttempted |
| PR40A_V28 | 100 | Blocked | Extend EZE runtime | 52 | 1125 | 0 | NotAttempted |
| PR43A_V42 | 100 | Blocked | Extend EZE runtime | 50 | 1466 | 0 | NotAttempted |
| PR44A_V13 | 100 | Blocked | Extend EZE runtime | 42 | 930 | 0 | NotAttempted |
| PR51A_V08 | 100 | Blocked | Extend EZE runtime | 11 | 315 | 0 | NotAttempted |
| PR52A_V08 | 100 | Blocked | Extend EZE runtime | 15 | 494 | 0 | NotAttempted |
| PR53A_V08 | 100 | Blocked | Extend EZE runtime | 11 | 390 | 0 | NotAttempted |
| PR56A_V28 | 100 | Blocked | Extend EZE runtime | 64 | 1721 | 0 | NotAttempted |
| PR57A_V02 | 100 | Blocked | Extend EZE runtime | 29 | 706 | 0 | NotAttempted |
| PR58A_V15 | 100 | Blocked | Extend EZE runtime | 42 | 1035 | 0 | NotAttempted |
| PR59A_V05 | 100 | Blocked | Extend EZE runtime | 25 | 690 | 0 | NotAttempted |
| PR60A_V09 | 100 | Blocked | Extend EZE runtime | 35 | 1123 | 0 | NotAttempted |
| PR61A_V04 | 100 | Blocked | Extend EZE runtime | 26 | 554 | 0 | NotAttempted |
| PR62A_V06 | 100 | Blocked | Extend EZE runtime | 20 | 388 | 0 | NotAttempted |
| PR65A_V03 | 100 | Blocked | Extend EZE runtime | 21 | 264 | 0 | NotAttempted |
| PR66A_V07 | 100 | Blocked | Extend EZE runtime | 54 | 924 | 0 | NotAttempted |
| PR67A_V03 | 100 | Blocked | Extend EZE runtime | 21 | 451 | 0 | NotAttempted |
| PR69A_V06 | 100 | Blocked | Extend EZE runtime | 27 | 738 | 0 | NotAttempted |
| PR70A_V15 | 100 | Blocked | Extend EZE runtime | 38 | 933 | 0 | NotAttempted |
| PR73A_V02 | 100 | Blocked | Extend EZE runtime | 41 | 905 | 0 | NotAttempted |
| PR75A_V08 | 100 | Blocked | Extend EZE runtime | 35 | 847 | 0 | NotAttempted |
| PR76A_V14 | 100 | Blocked | Extend EZE runtime | 21 | 924 | 0 | NotAttempted |
| PR88A_V44 | 100 | Blocked | Extend EZE runtime | 99 | 2714 | 0 | NotAttempted |
| PZ04A_V02 | 100 | Blocked | Extend statement parser | 16 | 192 | 0,8 | NotAttempted |
| PZ05A_V02 | 100 | Blocked | Extend EZE runtime | 17 | 189 | 0 | NotAttempted |
| PZ06A_V02 | 100 | Blocked | Extend EZE runtime | 17 | 189 | 0 | NotAttempted |
| PZ07A_V02 | 100 | Blocked | Extend EZE runtime | 19 | 208 | 0 | NotAttempted |
| PZ08A_V02 | 100 | Blocked | Extend EZE runtime | 42 | 373 | 0 | NotAttempted |
| PZ09A_V02 | 100 | Blocked | Extend EZE runtime | 34 | 360 | 0 | NotAttempted |
| PZ10A_V02 | 100 | Blocked | Extend EZE runtime | 42 | 426 | 0 | NotAttempted |
| PZ11A_V02 | 100 | Blocked | Extend EZE runtime | 16 | 184 | 0 | NotAttempted |
| PZ12A_V02 | 100 | Blocked | Extend EZE runtime | 16 | 184 | 0 | NotAttempted |
| PZ15A_V02 | 100 | Blocked | Extend EZE runtime | 16 | 206 | 0 | NotAttempted |
| PZ16A_V02 | 100 | Blocked | Extend EZE runtime | 16 | 184 | 0 | NotAttempted |
| PZ17A_V02 | 100 | Blocked | Extend EZE runtime | 42 | 368 | 0 | NotAttempted |
| PZ18A_V02 | 100 | Blocked | Extend EZE runtime | 16 | 184 | 0 | NotAttempted |
| PZ19A_V02 | 100 | Blocked | Extend EZE runtime | 16 | 186 | 0 | NotAttempted |
| PZ20A_V02 | 100 | Blocked | Extend EZE runtime | 18 | 205 | 0 | NotAttempted |
| PZ21_V02 | 100 | Blocked | Extend EZE runtime | 16 | 198 | 0 | NotAttempted |
| RE02A_V38 | 100 | Blocked | Extend EZE runtime | 66 | 1748 | 0 | NotAttempted |
| RE03A_V24 | 100 | Blocked | Extend EZE runtime | 45 | 877 | 0 | NotAttempted |
| RE05A_V30 | 100 | Blocked | Extend EZE runtime | 84 | 1653 | 0 | NotAttempted |
| RE06A_V28 | 100 | Blocked | Extend EZE runtime | 52 | 875 | 0 | NotAttempted |
| RE09A_V07 | 100 | Blocked | Extend EZE runtime | 31 | 563 | 0 | NotAttempted |
| RE11A_V05 | 100 | Blocked | Extend EZE runtime | 17 | 479 | 0 | NotAttempted |
| RE13A_V05 | 100 | Blocked | Extend EZE runtime | 34 | 647 | 0 | NotAttempted |
| RE19A_V05 | 100 | Blocked | Extend EZE runtime | 36 | 973 | 0 | NotAttempted |
| RE32A_V12 | 100 | Blocked | Extend EZE runtime | 65 | 1736 | 0 | NotAttempted |
| RE33A_V18 | 100 | Blocked | Extend EZE runtime | 46 | 824 | 0 | NotAttempted |
| RE35A_V15 | 100 | Blocked | Extend EZE runtime | 81 | 1583 | 0 | NotAttempted |
| RE36A_V07 | 100 | Blocked | Extend EZE runtime | 50 | 799 | 0 | NotAttempted |
| RE38A_V04 | 100 | Blocked | Extend EZE runtime | 28 | 612 | 0 | NotAttempted |
| RE39A_V04 | 100 | Blocked | Extend EZE runtime | 28 | 574 | 0 | NotAttempted |
| RE82A_V37 | 100 | Blocked | Extend EZE runtime | 81 | 2380 | 0 | NotAttempted |
| RE83A_V36 | 100 | Blocked | Extend EZE runtime | 63 | 1725 | 0 | NotAttempted |
| RE85A_V31 | 100 | Blocked | Extend EZE runtime | 92 | 2132 | 0 | NotAttempted |
| RE86A_V21 | 100 | Blocked | Extend EZE runtime | 48 | 1116 | 0 | NotAttempted |
| RO11A_V12 | 100 | Blocked | Extend EZE runtime | 39 | 994 | 0 | NotAttempted |
| RO23A_V30A | 100 | Blocked | Extend EZE runtime | 31 | 746 | 0 | NotAttempted |
| RO23A_V31 | 100 | Blocked | Extend EZE runtime | 31 | 746 | 0 | NotAttempted |
| RO24A_V42A | 100 | Blocked | Extend EZE runtime | 43 | 1302 | 0 | NotAttempted |
| RO24A_V42B | 100 | Blocked | Extend EZE runtime | 43 | 1302 | 0 | NotAttempted |
| RO24A_V43 | 100 | Blocked | Extend EZE runtime | 43 | 1302 | 0 | NotAttempted |
| RO41A_V29 | 100 | Blocked | Extend EZE runtime | 37 | 898 | 0 | NotAttempted |
| RO43A_V13 | 100 | Blocked | Extend EZE runtime | 18 | 463 | 0 | NotAttempted |
| RO55A_V10 | 100 | Blocked | Extend EZE runtime | 32 | 1028 | 0 | NotAttempted |
| S051A_V08 | 100 | Blocked | Extend EZE runtime | 27 | 678 | 0 | NotAttempted |
| S056A_V03_NT | 100 | Blocked | Extend EZE runtime | 108 | 2184 | 0 | NotAttempted |
| S056A_V04 | 100 | Blocked | Extend EZE runtime | 27 | 546 | 0 | NotAttempted |
| S101A_V04 | 100 | Blocked | Extend EZE runtime | 34 | 699 | 0 | NotAttempted |
| S114A_V08 | 100 | Blocked | Extend EZE runtime | 22 | 447 | 0 | NotAttempted |
| S116A_V11 | 100 | Blocked | Extend EZE runtime | 27 | 641 | 0 | NotAttempted |
| S150A_V03 | 100 | Blocked | Extend EZE runtime | 23 | 599 | 0 | NotAttempted |
| S202A_V06 | 100 | Blocked | Extend EZE runtime | 19 | 500 | 0 | NotAttempted |
| S204A_V11 | 100 | Blocked | Extend EZE runtime | 15 | 426 | 0 | NotAttempted |
| S211A_V08 | 100 | Blocked | Extend EZE runtime | 34 | 914 | 0 | NotAttempted |
| S214A_V16 | 100 | Blocked | Extend EZE runtime | 25 | 656 | 0 | NotAttempted |
| S215A_V25_EUR | 100 | Blocked | Extend EZE runtime | 30 | 863 | 0 | NotAttempted |
| S215A_V30 | 100 | Blocked | Extend EZE runtime | 32 | 944 | 0 | NotAttempted |
| S216A_V17 | 100 | Blocked | Extend EZE runtime | 19 | 545 | 0 | NotAttempted |
| S218A_V08 | 100 | Blocked | Extend EZE runtime | 18 | 381 | 0 | NotAttempted |
| S219A_V29_EUR | 100 | Blocked | Extend statement parser | 81 | 3082 | 0,3 | NotAttempted |
| S219A_V30_EUR | 100 | Blocked | Extend statement parser | 81 | 3082 | 0,3 | NotAttempted |
| S219A_V43 | 100 | Blocked | Extend statement parser | 86 | 3270 | 0,3 | NotAttempted |
| S220A_V07_EUR | 100 | Blocked | Extend statement parser | 30 | 1137 | 0,9 | NotAttempted |
| S220A_V09 | 100 | Blocked | Extend statement parser | 30 | 1137 | 0,9 | NotAttempted |
| S221A_V03 | 100 | Blocked | Extend EZE runtime | 17 | 460 | 0 | NotAttempted |
| S234A_V03 | 100 | Blocked | Extend EZE runtime | 32 | 871 | 0 | NotAttempted |
| S402A_V04 | 100 | Blocked | Extend EZE runtime | 26 | 561 | 0 | NotAttempted |
| S404A_V04 | 100 | Blocked | Extend EZE runtime | 18 | 456 | 0 | NotAttempted |
| S502A_V11 | 100 | Blocked | Extend EZE runtime | 31 | 650 | 0 | NotAttempted |
| S507A_V15 | 100 | Blocked | Extend EZE runtime | 32 | 909 | 0 | NotAttempted |
| S508A_V34 | 100 | Blocked | Extend EZE runtime | 73 | 2631 | 0 | NotAttempted |
| S510A_V08 | 100 | Blocked | Extend EZE runtime | 29 | 833 | 0 | NotAttempted |
| S513A_V41 | 100 | Blocked | Extend EZE runtime | 62 | 1475 | 0 | NotAttempted |
| S518A_V20 | 100 | Blocked | Extend EZE runtime | 46 | 1436 | 0 | NotAttempted |
| S528A_V15A | 100 | Blocked | Extend EZE runtime | 42 | 1222 | 0 | NotAttempted |
| S528A_V38 | 100 | Blocked | Extend EZE runtime | 76 | 2623 | 0 | NotAttempted |
| S530A_V05 | 100 | Blocked | Extend EZE runtime | 22 | 769 | 0 | NotAttempted |
| S532A_V20 | 100 | Blocked | Extend EZE runtime | 49 | 797 | 0 | NotAttempted |
| S533A_V18 | 100 | Blocked | Extend EZE runtime | 36 | 1050 | 0 | NotAttempted |
| S536A_V10 | 100 | Blocked | Extend EZE runtime | 20 | 636 | 0 | NotAttempted |
| S538A_V02 | 100 | Blocked | Extend EZE runtime | 14 | 350 | 0 | NotAttempted |
| S540A_V03 | 100 | Blocked | Extend EZE runtime | 13 | 171 | 0 | NotAttempted |
| S546A_V21 | 100 | Blocked | Extend EZE runtime | 95 | 2743 | 0 | NotAttempted |
| S547A_V10 | 100 | Blocked | Extend EZE runtime | 72 | 2249 | 0 | NotAttempted |
| S549A_V13 | 100 | Blocked | Extend EZE runtime | 68 | 1471 | 0 | NotAttempted |
| S550A_V21 | 100 | Blocked | Extend EZE runtime | 48 | 919 | 0 | NotAttempted |
| S551A_V07 | 100 | Blocked | Extend EZE runtime | 35 | 931 | 0 | NotAttempted |
| S564_V07 | 100 | Blocked | Extend EZE runtime | 42 | 813 | 0 | NotAttempted |
| S566_V12 | 100 | Blocked | Extend EZE runtime | 59 | 1445 | 0 | NotAttempted |
| S569_V08 | 100 | Blocked | Extend EZE runtime | 61 | 1440 | 0 | NotAttempted |
| S575_V09 | 100 | Blocked | Extend EZE runtime | 47 | 961 | 0 | NotAttempted |
| S576_V06 | 100 | Blocked | Extend EZE runtime | 33 | 911 | 0 | NotAttempted |
| S585_V20 | 100 | Blocked | Extend EZE runtime | 78 | 2227 | 0 | NotAttempted |
| S586_V06 | 100 | Blocked | Extend EZE runtime | 16 | 515 | 0 | NotAttempted |
| S589_V05 | 100 | Blocked | Extend EZE runtime | 16 | 333 | 0 | NotAttempted |
| S590_V04 | 100 | Blocked | Extend EZE runtime | 22 | 466 | 0 | NotAttempted |
| S592A_V11 | 100 | Blocked | Extend EZE runtime | 39 | 836 | 0 | NotAttempted |
| S592_V08 | 100 | Blocked | Extend EZE runtime | 185 | 4085 | 0 | NotAttempted |
| S594_V14 | 100 | Blocked | Extend EZE runtime | 65 | 1805 | 0 | NotAttempted |
| S595_V04 | 100 | Blocked | Extend EZE runtime | 41 | 901 | 0 | NotAttempted |
| S599_V08 | 100 | Blocked | Extend EZE runtime | 45 | 1545 | 0 | NotAttempted |
| S601A_V19 | 100 | Blocked | Extend EZE runtime | 26 | 698 | 0 | NotAttempted |
| S601_V18 | 100 | Blocked | Extend EZE runtime | 26 | 694 | 0 | NotAttempted |
| S602_V05 | 100 | Blocked | Extend EZE runtime | 19 | 332 | 0 | NotAttempted |
| S603_V10 | 100 | Blocked | Extend EZE runtime | 62 | 1912 | 0 | NotAttempted |
| S605_V06 | 100 | Blocked | Extend EZE runtime | 36 | 875 | 0 | NotAttempted |
| S607_V13 | 100 | Blocked | Extend EZE runtime | 85 | 2531 | 0 | NotAttempted |
| S608_V05 | 100 | Blocked | Extend EZE runtime | 30 | 839 | 0 | NotAttempted |
| S609_V08 | 100 | Blocked | Extend EZE runtime | 25 | 510 | 0 | NotAttempted |
| S610_V15 | 100 | Blocked | Extend EZE runtime | 71 | 1911 | 0 | NotAttempted |
| S612_V05 | 100 | Blocked | Extend EZE runtime | 28 | 816 | 0 | NotAttempted |
| S615A_V15 | 100 | Blocked | Extend EZE runtime | 37 | 1285 | 0 | NotAttempted |
| S618_V05 | 100 | Blocked | Extend EZE runtime | 24 | 527 | 0 | NotAttempted |
| S619A_V02 | 100 | Blocked | Extend EZE runtime | 46 | 938 | 0 | NotAttempted |
| S619_V02 | 100 | Blocked | Extend EZE runtime | 46 | 938 | 0 | NotAttempted |
| S620A_V02 | 100 | Blocked | Extend EZE runtime | 17 | 294 | 0 | NotAttempted |
| S621A_V10 | 100 | Blocked | Extend EZE runtime | 49 | 1066 | 0 | NotAttempted |
| S622A_V03 | 100 | Blocked | Extend EZE runtime | 41 | 1286 | 0 | NotAttempted |
| S700A_V16 | 100 | Blocked | Extend EZE runtime | 38 | 607 | 0 | NotAttempted |
| S702A_V07 | 100 | Blocked | Extend EZE runtime | 43 | 1138 | 0 | NotAttempted |
| S704A_V18 | 100 | Blocked | Extend EZE runtime | 63 | 1784 | 0 | NotAttempted |
| S710A_V21 | 100 | Blocked | Extend EZE runtime | 93 | 1818 | 0 | NotAttempted |
| S712A_V22 | 100 | Blocked | Extend EZE runtime | 134 | 4149 | 0 | NotAttempted |
| S713A_V17 | 100 | Blocked | Extend EZE runtime | 109 | 2577 | 0 | NotAttempted |
| S714A_V04 | 100 | Blocked | Extend EZE runtime | 66 | 1164 | 0 | NotAttempted |
| S721A_V02 | 100 | Blocked | Extend EZE runtime | 30 | 648 | 0 | NotAttempted |
| S725A_V07 | 100 | Blocked | Extend EZE runtime | 36 | 811 | 0 | NotAttempted |
| S730A_V38 | 100 | Blocked | Extend EZE runtime | 165 | 6704 | 0 | NotAttempted |
| S736A_V04 | 100 | Blocked | Extend EZE runtime | 24 | 603 | 0 | NotAttempted |
| S737A_V01 | 100 | Blocked | Extend EZE runtime | 25 | 473 | 0 | NotAttempted |
| S745A_V01 | 100 | Blocked | Extend EZE runtime | 15 | 227 | 0 | NotAttempted |
| S746A_V30A | 100 | Blocked | Extend EZE runtime | 102 | 2911 | 0 | NotAttempted |
| S746A_V34 | 100 | Blocked | Extend EZE runtime | 108 | 3073 | 0 | NotAttempted |
| S803A_V17 | 100 | Blocked | Extend EZE runtime | 86 | 1741 | 0 | NotAttempted |
| SA20A_V18 | 100 | Blocked | Extend EZE runtime | 34 | 899 | 0 | NotAttempted |
| SA40A_V15 | 100 | Blocked | Extend EZE runtime | 172 | 6576 | 0 | NotAttempted |
| SA45A_V56 | 100 | Blocked | Extend EZE runtime | 120 | 3799 | 0 | NotAttempted |
| SE14A_V53 | 100 | Blocked | Extend EZE runtime | 69 | 2360 | 0 | NotAttempted |
| SE19A_V10 | 100 | Blocked | Extend EZE runtime | 45 | 1276 | 0 | NotAttempted |
| SE20 | 100 | Blocked | Extend EZE runtime | 65 | 1917 | 0 | NotAttempted |
| SE20A_V49 | 100 | Blocked | Extend EZE runtime | 67 | 1966 | 0 | NotAttempted |
| SE21A_V11 | 100 | Blocked | Extend EZE runtime | 30 | 913 | 0 | NotAttempted |
| SE22A_V44 | 100 | Blocked | Extend EZE runtime | 59 | 2311 | 0 | NotAttempted |
| SE24A_V14 | 100 | Blocked | Extend EZE runtime | 28 | 614 | 0 | NotAttempted |
| SE26A_V42 | 100 | Blocked | Extend EZE runtime | 57 | 1525 | 0 | NotAttempted |
| SE28A_V06 | 100 | Blocked | Extend EZE runtime | 30 | 813 | 0 | NotAttempted |
| SE30A_V25 | 100 | Blocked | Extend EZE runtime | 169 | 6913 | 0 | NotAttempted |
| SE31A_V06 | 100 | Blocked | Extend EZE runtime | 41 | 1136 | 0 | NotAttempted |
| SE33A_V05 | 100 | Blocked | Extend EZE runtime | 25 | 637 | 0 | NotAttempted |
| SL | 100 | Blocked | Extend EZE runtime | 77 | 1683 | 0 | NotAttempted |
| SL01A_V29 | 100 | Blocked | Extend EZE runtime | 147 | 2860 | 0 | NotAttempted |
| SL03A_V12 | 100 | Blocked | Extend EZE runtime | 104 | 1669 | 0 | NotAttempted |
| SL08A_V29 | 100 | Blocked | Extend EZE runtime | 143 | 3415 | 0 | NotAttempted |
| SL10A_V42A | 100 | Blocked | Extend EZE runtime | 83 | 1874 | 0 | NotAttempted |
| SL10A_V44 | 100 | Blocked | Extend EZE runtime | 83 | 1888 | 0 | NotAttempted |
| SL13A_V20 | 100 | Blocked | Extend EZE runtime | 44 | 1067 | 0 | NotAttempted |
| SL15A_V08 | 100 | Blocked | Extend EZE runtime | 31 | 606 | 0 | NotAttempted |
| SU11A_V08 | 100 | Blocked | Extend EZE runtime | 30 | 619 | 0 | NotAttempted |
| SU22A_04 | 100 | Blocked | Extend EZE runtime | 24 | 401 | 0 | NotAttempted |
| TE18A_V08 | 100 | Blocked | Extend EZE runtime | 14 | 306 | 0 | NotAttempted |
| TE19A_V08 | 100 | Blocked | Extend EZE runtime | 16 | 460 | 0 | NotAttempted |
| TE23A_V07 | 100 | Blocked | Extend EZE runtime | 20 | 466 | 0 | NotAttempted |
| TE38A_V16 | 100 | Blocked | Extend EZE runtime | 174 | 4976 | 0 | NotAttempted |
| TE40A_V19 | 100 | Blocked | Extend EZE runtime | 40 | 1509 | 0 | NotAttempted |
| TE56A_V20 | 100 | Blocked | Extend EZE runtime | 55 | 2012 | 0 | NotAttempted |
| TE60A_V14 | 100 | Blocked | Extend EZE runtime | 66 | 5156 | 0 | NotAttempted |
| TE61A_V19 | 100 | Blocked | Extend EZE runtime | 92 | 2758 | 0 | NotAttempted |
| TM01A_V49 | 100 | Blocked | Extend EZE runtime | 81 | 2734 | 0 | NotAttempted |
| TM02A_V19 | 100 | Blocked | Extend EZE runtime | 26 | 453 | 0 | NotAttempted |
| TM04A_V17 | 100 | Blocked | Extend EZE runtime | 31 | 626 | 0 | NotAttempted |
| TM05A_V16 | 100 | Blocked | Extend EZE runtime | 20 | 341 | 0 | NotAttempted |
| ZZ51A_V04 | 100 | Blocked | Extend EZE runtime | 38 | 687 | 0 | NotAttempted |
| ZZ96A_V19 | 100 | Blocked | Extend EZE runtime | 37 | 824 | 0 | NotAttempted |
| ZZ97A_V07 | 100 | Blocked | Extend EZE runtime | 61 | 1407 | 0 | NotAttempted |
| ZZ98A_V40 | 100 | Blocked | Extend EZE runtime | 220 | 5091 | 0 | NotAttempted |
| MK12A_V06 | 95 | Blocked | Extend EZE runtime | 25 | 497 | 0 | NotAttempted |
| S005A_V05 | 95 | Blocked | Extend EZE runtime | 16 | 386 | 0 | NotAttempted |
| S152A_V08 | 95 | Blocked | Extend EZE runtime | 16 | 385 | 0 | NotAttempted |
| S563_V05 | 95 | Blocked | Extend statement parser | 10 | 266 | 4,6 | NotAttempted |
| SE03A_V05 | 95 | Blocked | Extend EZE runtime | 22 | 473 | 0 | NotAttempted |
| SU16A_V03 | 95 | Blocked | Extend EZE runtime | 12 | 265 | 0 | NotAttempted |
| CT01A_V07 | 90 | Blocked | Extend EZE runtime | 20 | 416 | 0 | NotAttempted |
| CT02A_V11 | 90 | Blocked | Extend EZE runtime | 26 | 569 | 0 | NotAttempted |
| CT03A_V03 | 90 | Blocked | Extend EZE runtime | 15 | 311 | 0 | NotAttempted |
| CT04A_V13 | 90 | Blocked | Extend EZE runtime | 26 | 578 | 0 | NotAttempted |
| CT05A_V03 | 90 | Blocked | Extend EZE runtime | 18 | 374 | 0 | NotAttempted |
| CT06A_V06 | 90 | Blocked | Extend EZE runtime | 26 | 580 | 0 | NotAttempted |
| CT07A_V15 | 90 | Blocked | Extend EZE runtime | 30 | 698 | 0 | NotAttempted |
| CT08A_V15 | 90 | Blocked | Extend EZE runtime | 23 | 577 | 0 | NotAttempted |
| CT09A_V08 | 90 | Blocked | Extend EZE runtime | 28 | 637 | 0 | NotAttempted |
| CT10A_V05 | 90 | Blocked | Extend EZE runtime | 22 | 447 | 0 | NotAttempted |
| CT12A_V04 | 90 | Blocked | Extend EZE runtime | 22 | 467 | 0 | NotAttempted |
| CT13A_V04 | 90 | Blocked | Extend EZE runtime | 21 | 498 | 0 | NotAttempted |
| CT14A_V05 | 90 | Blocked | Extend EZE runtime | 24 | 521 | 0 | NotAttempted |
| CT15A_V17 | 90 | Blocked | Extend EZE runtime | 28 | 715 | 0 | NotAttempted |
| CT18A_V10 | 90 | Blocked | Extend EZE runtime | 25 | 605 | 0 | NotAttempted |
| CT19A_V09 | 90 | Blocked | Extend EZE runtime | 22 | 532 | 0 | NotAttempted |
| CT20A_V06 | 90 | Blocked | Extend EZE runtime | 23 | 666 | 0 | NotAttempted |
| CT21A_V16 | 90 | Blocked | Extend EZE runtime | 27 | 648 | 0 | NotAttempted |
| CT22A_V04 | 90 | Blocked | Extend EZE runtime | 26 | 490 | 0 | NotAttempted |
| CT23A_V02 | 90 | Blocked | Extend EZE runtime | 19 | 380 | 0 | NotAttempted |
| CT24A_V04 | 90 | Blocked | Extend EZE runtime | 21 | 507 | 0 | NotAttempted |
| CT29A_V09 | 90 | Blocked | Extend EZE runtime | 25 | 614 | 0 | NotAttempted |
| CT30A_V02 | 90 | Blocked | Extend EZE runtime | 15 | 288 | 0 | NotAttempted |
| CT31A_V02 | 90 | Blocked | Extend EZE runtime | 17 | 329 | 0 | NotAttempted |
| CT33A_V08 | 90 | Blocked | Extend EZE runtime | 34 | 722 | 0 | NotAttempted |
| CT34A_V04 | 90 | Blocked | Extend EZE runtime | 24 | 507 | 0 | NotAttempted |
| D003A_V05 | 90 | Blocked | Extend EZE runtime | 10 | 293 | 0 | NotAttempted |
| D005A_V04 | 90 | Blocked | Extend EZE runtime | 10 | 307 | 0 | NotAttempted |
| D007A_V42 | 90 | Blocked | Extend EZE runtime | 63 | 1707 | 0 | NotAttempted |
| D008A_V04 | 90 | Blocked | Extend EZE runtime | 13 | 418 | 0 | NotAttempted |
| D013A_V03 | 90 | Blocked | Extend EZE runtime | 13 | 372 | 0 | NotAttempted |
| D020A_V11 | 90 | Blocked | Extend EZE runtime | 30 | 989 | 0 | NotAttempted |
| D021A_V18 | 90 | Blocked | Extend EZE runtime | 50 | 1243 | 0 | NotAttempted |
| D022A_V07 | 90 | Blocked | Extend EZE runtime | 19 | 721 | 0 | NotAttempted |
| D100A_V58 | 90 | Blocked | Extend EZE runtime | 101 | 3491 | 0 | NotAttempted |
| D102A_V35 | 90 | Blocked | Extend EZE runtime | 225 | 8250 | 0 | NotAttempted |
| D104A_V07 | 90 | Blocked | Extend EZE runtime | 28 | 596 | 0 | NotAttempted |
| D105A_V28 | 90 | Blocked | Extend EZE runtime | 46 | 1388 | 0 | NotAttempted |
| D106A_V29 | 90 | Blocked | Extend EZE runtime | 103 | 3650 | 0 | NotAttempted |
| D108A_V91 | 90 | Blocked | Extend EZE runtime | 122 | 4717 | 0 | NotAttempted |
| D109A_V16 | 90 | Blocked | Extend EZE runtime | 42 | 1090 | 0 | NotAttempted |
| D111A_V29 | 90 | Blocked | Extend EZE runtime | 123 | 5154 | 0 | NotAttempted |
| D112A_V06 | 90 | Blocked | Extend EZE runtime | 16 | 466 | 0 | NotAttempted |
| D114A_V18 | 90 | Blocked | Extend EZE runtime | 37 | 1191 | 0 | NotAttempted |
| D115A_V34 | 90 | Blocked | Extend statement parser | 78 | 3148 | 0,3 | NotAttempted |
| D122A_V49 | 90 | Blocked | Extend statement parser | 110 | 3995 | 0,2 | NotAttempted |
| D123A_V55 | 90 | Blocked | Extend EZE runtime | 67 | 2103 | 0 | NotAttempted |
| D128A_V11 | 90 | Blocked | Extend EZE runtime | 23 | 523 | 0 | NotAttempted |
| D132A_V83 | 90 | Blocked | Extend EZE runtime | 117 | 4498 | 0 | NotAttempted |
| D133A_V70 | 90 | Blocked | Extend EZE runtime | 112 | 2879 | 0 | NotAttempted |
| D138A_V34 | 90 | Blocked | Extend EZE runtime | 66 | 2452 | 0 | NotAttempted |
| D140A_V22 | 90 | Blocked | Extend EZE runtime | 27 | 506 | 0 | NotAttempted |
| D144A_V84 | 90 | Blocked | Extend EZE runtime | 122 | 4279 | 0 | NotAttempted |
| D149A_V13 | 90 | Blocked | Extend EZE runtime | 30 | 755 | 0 | NotAttempted |
| D151A_V10 | 90 | Blocked | Extend EZE runtime | 62 | 1514 | 0 | NotAttempted |
| D152A_V37 | 90 | Blocked | Extend EZE runtime | 80 | 2087 | 0 | NotAttempted |
| D158A_V04 | 90 | Blocked | Extend EZE runtime | 19 | 344 | 0 | NotAttempted |
| D160A_V03 | 90 | Blocked | Extend EZE runtime | 25 | 734 | 0 | NotAttempted |
| D163A_V23 | 90 | Blocked | Extend EZE runtime | 34 | 846 | 0 | NotAttempted |
| D167A_V08 | 90 | Blocked | Extend EZE runtime | 34 | 945 | 0 | NotAttempted |
| D169A_V33 | 90 | Blocked | Extend EZE runtime | 52 | 1980 | 0 | NotAttempted |
| D178A_V04 | 90 | Blocked | Extend EZE runtime | 47 | 1416 | 0 | NotAttempted |
| D179A_V45 | 90 | Blocked | Extend EZE runtime | 67 | 2156 | 0 | NotAttempted |
| D195A_V28 | 90 | Blocked | Extend EZE runtime | 71 | 1981 | 0 | NotAttempted |
| D201A_V18 | 90 | Blocked | Extend EZE runtime | 41 | 850 | 0 | NotAttempted |
| D204A_V29 | 90 | Blocked | Extend EZE runtime | 69 | 1990 | 0 | NotAttempted |
| D207A_V31 | 90 | Blocked | Extend EZE runtime | 100 | 2636 | 0 | NotAttempted |
| D208A_V10 | 90 | Blocked | Extend EZE runtime | 53 | 1231 | 0 | NotAttempted |
| D210A_V06 | 90 | Blocked | Extend EZE runtime | 36 | 854 | 0 | NotAttempted |
| D212A_V43 | 90 | Blocked | Extend EZE runtime | 74 | 2465 | 0 | NotAttempted |
| D215A_V23 | 90 | Blocked | Extend EZE runtime | 75 | 2442 | 0 | NotAttempted |
| D217A_V54 | 90 | Blocked | Extend EZE runtime | 80 | 2112 | 0 | NotAttempted |
| D219A_V17 | 90 | Blocked | Extend EZE runtime | 49 | 1363 | 0 | NotAttempted |
| D223A_V01 | 90 | Blocked | Extend EZE runtime | 24 | 450 | 0 | NotAttempted |
| D224A_V05 | 90 | Blocked | Extend EZE runtime | 32 | 715 | 0 | NotAttempted |
| D231A_V13 | 90 | Blocked | Extend EZE runtime | 53 | 1710 | 0 | NotAttempted |
| D235A_V08 | 90 | Blocked | Extend EZE runtime | 38 | 888 | 0 | NotAttempted |
| D236A_V09 | 90 | Blocked | Extend EZE runtime | 35 | 864 | 0 | NotAttempted |
| D242A_V17 | 90 | Blocked | Extend EZE runtime | 56 | 1569 | 0 | NotAttempted |
| D243A_V04 | 90 | Blocked | Extend EZE runtime | 23 | 499 | 0 | NotAttempted |
| D244A_V14 | 90 | Blocked | Extend EZE runtime | 58 | 1629 | 0 | NotAttempted |
| D245A_V15 | 90 | Blocked | Extend EZE runtime | 52 | 1475 | 0 | NotAttempted |
| D246A_V01 | 90 | Blocked | Extend EZE runtime | 28 | 499 | 0 | NotAttempted |
| D251A_V04 | 90 | Blocked | Extend EZE runtime | 22 | 402 | 0 | NotAttempted |
| D257A_V07 | 90 | Blocked | Extend EZE runtime | 47 | 1104 | 0 | NotAttempted |
| D258A_V14 | 90 | Blocked | Extend EZE runtime | 50 | 916 | 0 | NotAttempted |
| D262A_V02 | 90 | Blocked | Extend EZE runtime | 27 | 472 | 0 | NotAttempted |
| D263A_V13 | 90 | Blocked | Extend EZE runtime | 90 | 1743 | 0 | NotAttempted |
| D268A_V06 | 90 | Blocked | Extend EZE runtime | 35 | 869 | 0 | NotAttempted |
| D269A_V10 | 90 | Blocked | Extend EZE runtime | 36 | 1113 | 0 | NotAttempted |
| D271A_V01 | 90 | Blocked | Extend EZE runtime | 28 | 577 | 0 | NotAttempted |
| D272A_V13 | 90 | Blocked | Extend EZE runtime | 79 | 1727 | 0 | NotAttempted |
| D276A_V18 | 90 | Blocked | Extend EZE runtime | 41 | 825 | 0 | NotAttempted |
| D277A_V01 | 90 | Blocked | Extend EZE runtime | 34 | 608 | 0 | NotAttempted |
| D287A_V04 | 90 | Blocked | Extend EZE runtime | 16 | 567 | 0 | NotAttempted |
| D289A_V13 | 90 | Blocked | Extend EZE runtime | 51 | 1313 | 0 | NotAttempted |
| D296A_V03 | 90 | Blocked | Extend EZE runtime | 17 | 329 | 0 | NotAttempted |
| D299A_V01 | 90 | Blocked | Extend EZE runtime | 31 | 497 | 0 | NotAttempted |
| D301A_V01 | 90 | Blocked | Extend EZE runtime | 9 | 213 | 0 | NotAttempted |
| D309A_V04 | 90 | Blocked | Extend EZE runtime | 27 | 547 | 0 | NotAttempted |
| D316A_V02 | 90 | Blocked | Extend EZE runtime | 28 | 516 | 0 | NotAttempted |
| D320 | 90 | Blocked | Extend EZE runtime | 43 | 672 | 0 | NotAttempted |
| D321A_V01 | 90 | Blocked | Extend EZE runtime | 43 | 683 | 0 | NotAttempted |
| D324A_V09 | 90 | Blocked | Extend EZE runtime | 65 | 1601 | 0 | NotAttempted |
| D328A_V03 | 90 | Blocked | Extend EZE runtime | 58 | 1142 | 0 | NotAttempted |
| D330A_V11 | 90 | Blocked | Extend EZE runtime | 52 | 1130 | 0 | NotAttempted |
| D331A_V02 | 90 | Blocked | Extend EZE runtime | 45 | 867 | 0 | NotAttempted |
| D335A_V02 | 90 | Blocked | Extend EZE runtime | 34 | 699 | 0 | NotAttempted |
| D342A_V04 | 90 | Blocked | Extend EZE runtime | 15 | 303 | 0 | NotAttempted |
| D343A_V02 | 90 | Blocked | Extend EZE runtime | 16 | 282 | 0 | NotAttempted |
| DE00A_V09 | 90 | Blocked | Extend EZE runtime | 15 | 482 | 0 | NotAttempted |
| DL11A_V37 | 90 | Blocked | Extend EZE runtime | 43 | 1618 | 0 | NotAttempted |
| DL14A_V13 | 90 | Blocked | Extend EZE runtime | 33 | 1187 | 0 | NotAttempted |
| DL17A_V05 | 90 | Blocked | Extend EZE runtime | 111 | 3999 | 0 | NotAttempted |
| DL25A_V09 | 90 | Blocked | Extend EZE runtime | 23 | 791 | 0 | NotAttempted |
| DL31A_V31 | 90 | Blocked | Extend EZE runtime | 65 | 1410 | 0 | NotAttempted |
| DL31A_V32_EUR | 90 | Blocked | Extend EZE runtime | 67 | 1439 | 0 | NotAttempted |
| DL31A_V32_MATEJ | 90 | Blocked | Extend EZE runtime | 65 | 1416 | 0 | NotAttempted |
| DL32A_V19 | 90 | Blocked | Extend EZE runtime | 46 | 963 | 0 | NotAttempted |
| DL53A_V30 | 90 | Blocked | Extend EZE runtime | 48 | 1340 | 0 | NotAttempted |
| DL91A_V20 | 90 | Blocked | Extend EZE runtime | 21 | 638 | 0 | NotAttempted |
| F238A_V16 | 90 | Blocked | Extend EZE runtime | 69 | 1968 | 0 | NotAttempted |
| F310A_V04 | 90 | Blocked | Extend EZE runtime | 43 | 674 | 0 | NotAttempted |
| IN81A_V09 | 90 | Blocked | Extend EZE runtime | 23 | 461 | 0 | NotAttempted |
| IS00A_V13A | 90 | Blocked | Extend EZE runtime | 34 | 1013 | 0 | NotAttempted |
| IS00A_V13B | 90 | Blocked | Extend EZE runtime | 34 | 1014 | 0 | NotAttempted |
| IS00A_V15B | 90 | Blocked | Extend EZE runtime | 34 | 1033 | 0 | NotAttempted |
| IS00A_V26 | 90 | Blocked | Extend EZE runtime | 34 | 1153 | 0 | NotAttempted |
| KA34A_V05_EUR | 90 | Blocked | Extend EZE runtime | 27 | 456 | 0 | NotAttempted |
| KA34A_V09 | 90 | Blocked | Extend EZE runtime | 28 | 457 | 0 | NotAttempted |
| KK01A_V18 | 90 | Blocked | Extend EZE runtime | 35 | 1467 | 0 | NotAttempted |
| KK02A_V02 | 90 | Blocked | Extend EZE runtime | 13 | 296 | 0 | NotAttempted |
| MK11A_V16 | 90 | Blocked | Extend EZE runtime | 23 | 500 | 0 | NotAttempted |
| NA42A_V62 | 90 | Blocked | Extend EZE runtime | 80 | 2518 | 0 | NotAttempted |
| NA56A_V36 | 90 | Blocked | Extend EZE runtime | 38 | 939 | 0 | NotAttempted |
| NA60A_V18 | 90 | Blocked | Extend EZE runtime | 30 | 764 | 0 | NotAttempted |
| NA75A_V30 | 90 | Blocked | Extend EZE runtime | 50 | 1921 | 0 | NotAttempted |
| NA80A_V11 | 90 | Blocked | Extend EZE runtime | 27 | 868 | 0 | NotAttempted |
| NA81A_V07 | 90 | Blocked | Extend EZE runtime | 91 | 2489 | 0 | NotAttempted |
| NA83A_V08 | 90 | Blocked | Extend EZE runtime | 44 | 1692 | 0 | NotAttempted |
| NR04A | 90 | Blocked | Extend EZE runtime | 44 | 906 | 0 | NotAttempted |
| PO54A_V11 | 90 | Blocked | Extend EZE runtime | 27 | 1629 | 0 | NotAttempted |
| PO54A_V12 | 90 | Blocked | Extend EZE runtime | 27 | 1630 | 0 | NotAttempted |
| PO54A_V14 | 90 | Blocked | Extend EZE runtime | 27 | 1661 | 0 | NotAttempted |
| PO54A_V15 | 90 | Blocked | Extend EZE runtime | 28 | 1684 | 0 | NotAttempted |
| PO54A_V16 | 90 | Blocked | Extend EZE runtime | 29 | 1728 | 0 | NotAttempted |
| PO54A_V17 | 90 | Blocked | Extend EZE runtime | 29 | 1748 | 0 | NotAttempted |
| PO54A_V18 | 90 | Blocked | Extend EZE runtime | 29 | 1872 | 0 | NotAttempted |
| PR42A_V12 | 90 | Blocked | Extend EZE runtime | 22 | 653 | 0 | NotAttempted |
| PR49A_V07 | 90 | Blocked | Extend EZE runtime | 13 | 357 | 0 | NotAttempted |
| PR50A_V23 | 90 | Blocked | Extend EZE runtime | 31 | 646 | 0 | NotAttempted |
| PR54A_V06 | 90 | Blocked | Extend EZE runtime | 18 | 388 | 0 | NotAttempted |
| PR55A_V03 | 90 | Blocked | Extend EZE runtime | 14 | 328 | 0 | NotAttempted |
| PR64A_V04 | 90 | Blocked | Extend EZE runtime | 17 | 297 | 0 | NotAttempted |
| PR68A_V13 | 90 | Blocked | Extend EZE runtime | 32 | 676 | 0 | NotAttempted |
| PR71A_V05 | 90 | Blocked | Extend EZE runtime | 8 | 227 | 0 | NotAttempted |
| PZ01A_V28_2 | 90 | Blocked | Extend EZE runtime | 51 | 1347 | 0 | NotAttempted |
| PZ01A_V41 | 90 | Blocked | Extend EZE runtime | 51 | 1346 | 0 | NotAttempted |
| PZ02A_V37 | 90 | Blocked | Extend EZE runtime | 51 | 1312 | 0 | NotAttempted |
| PZ02A_V38_1 | 90 | Blocked | Extend EZE runtime | 51 | 1312 | 0 | NotAttempted |
| PZ03A_V02 | 90 | Blocked | Extend EZE runtime | 17 | 164 | 0 | NotAttempted |
| RE10A_V03 | 90 | Blocked | Extend EZE runtime | 17 | 381 | 0 | NotAttempted |
| RE89A_V02 | 90 | Blocked | Extend EZE runtime | 26 | 889 | 0 | NotAttempted |
| RO05A_V07 | 90 | Blocked | Extend EZE runtime | 7 | 145 | 0 | NotAttempted |
| RO21A_V15 | 90 | Blocked | Extend EZE runtime | 28 | 791 | 0 | NotAttempted |
| RO33A_V04 | 90 | Blocked | Extend EZE runtime | 13 | 280 | 0 | NotAttempted |
| RO42A_V08 | 90 | Blocked | Extend EZE runtime | 18 | 418 | 0 | NotAttempted |
| S | 90 | Blocked | Extend EZE runtime | 47 | 1255 | 0 | NotAttempted |
| S212A_V09 | 90 | Blocked | Extend EZE runtime | 29 | 730 | 0 | NotAttempted |
| S501A_V33 | 90 | Blocked | Extend EZE runtime | 36 | 1316 | 0 | NotAttempted |
| S503A_V03 | 90 | Blocked | Extend EZE runtime | 14 | 347 | 0 | NotAttempted |
| S504A_V03 | 90 | Blocked | Extend EZE runtime | 13 | 288 | 0 | NotAttempted |
| S505A_V04 | 90 | Blocked | Extend EZE runtime | 52 | 1284 | 0 | NotAttempted |
| S506A_V24A | 90 | Blocked | Extend EZE runtime | 51 | 3406 | 0 | NotAttempted |
| S509A_V20 | 90 | Blocked | Extend EZE runtime | 82 | 2542 | 0 | NotAttempted |
| S515A_V24 | 90 | Blocked | Extend EZE runtime | 25 | 959 | 0 | NotAttempted |
| S516A_V25 | 90 | Blocked | Extend EZE runtime | 53 | 1411 | 0 | NotAttempted |
| S520A_V20 | 90 | Blocked | Extend EZE runtime | 25 | 866 | 0 | NotAttempted |
| S529A_V05 | 90 | Blocked | Extend EZE runtime | 26 | 624 | 0 | NotAttempted |
| S531A_V07 | 90 | Blocked | Extend EZE runtime | 20 | 425 | 0 | NotAttempted |
| S535A_V18_EUR | 90 | Blocked | Extend EZE runtime | 92 | 1894 | 0 | NotAttempted |
| S535A_V19_EUR | 90 | Blocked | Extend EZE runtime | 92 | 1896 | 0 | NotAttempted |
| S535A_V23 | 90 | Blocked | Extend EZE runtime | 93 | 2016 | 0 | NotAttempted |
| S539A_V06 | 90 | Blocked | Extend EZE runtime | 23 | 783 | 0 | NotAttempted |
| S545A_V18 | 90 | Blocked | Extend EZE runtime | 70 | 2350 | 0 | NotAttempted |
| S552A_V03 | 90 | Blocked | Extend EZE runtime | 32 | 546 | 0 | NotAttempted |
| S553A_V11 | 90 | Blocked | Extend EZE runtime | 37 | 1113 | 0 | NotAttempted |
| S554A_V05 | 90 | Blocked | Extend EZE runtime | 18 | 615 | 0 | NotAttempted |
| S555_V20 | 90 | Blocked | Extend EZE runtime | 54 | 2446 | 0 | NotAttempted |
| S556_V20 | 90 | Blocked | Extend EZE runtime | 45 | 2066 | 0 | NotAttempted |
| S557_V17 | 90 | Blocked | Extend statement parser | 70 | 2475 | 0,4 | NotAttempted |
| S558A_V37 | 90 | Blocked | Extend EZE runtime | 89 | 3161 | 0 | NotAttempted |
| S559A_V11 | 90 | Blocked | Extend statement parser | 70 | 2547 | 0,4 | NotAttempted |
| S559_V11 | 90 | Blocked | Extend statement parser | 70 | 2540 | 0,4 | NotAttempted |
| S561A_V05 | 90 | Blocked | Extend EZE runtime | 30 | 949 | 0 | NotAttempted |
| S561_V04 | 90 | Blocked | Extend EZE runtime | 56 | 1760 | 0 | NotAttempted |
| S562_V08 | 90 | Blocked | Extend EZE runtime | 28 | 404 | 0 | NotAttempted |
| S565_V09 | 90 | Blocked | Extend EZE runtime | 34 | 974 | 0 | NotAttempted |
| S568_V07 | 90 | Blocked | Extend EZE runtime | 34 | 1226 | 0 | NotAttempted |
| S570_V04 | 90 | Blocked | Extend EZE runtime | 24 | 760 | 0 | NotAttempted |
| S571_V04 | 90 | Blocked | Extend EZE runtime | 16 | 468 | 0 | NotAttempted |
| S572_V04 | 90 | Blocked | Extend EZE runtime | 23 | 825 | 0 | NotAttempted |
| S573_V15 | 90 | Blocked | Extend EZE runtime | 81 | 2811 | 0 | NotAttempted |
| S574_V19 | 90 | Blocked | Extend EZE runtime | 78 | 2340 | 0 | NotAttempted |
| S577_V02 | 90 | Blocked | Extend EZE runtime | 20 | 512 | 0 | NotAttempted |
| S578A_V46 | 90 | Blocked | Extend EZE runtime | 81 | 2459 | 0 | NotAttempted |
| S579A06 | 90 | Blocked | Extend EZE runtime | 24 | 683 | 0 | NotAttempted |
| S579_V11 | 90 | Blocked | Extend EZE runtime | 32 | 880 | 0 | NotAttempted |
| S582_V04 | 90 | Blocked | Extend EZE runtime | 14 | 292 | 0 | NotAttempted |
| S583_V10 | 90 | Blocked | Extend EZE runtime | 24 | 527 | 0 | NotAttempted |
| S588A_V06 | 90 | Blocked | Extend EZE runtime | 18 | 429 | 0 | NotAttempted |
| S591_V03 | 90 | Blocked | Extend EZE runtime | 8 | 335 | 0 | NotAttempted |
| S598_V06 | 90 | Blocked | Extend EZE runtime | 26 | 822 | 0 | NotAttempted |
| S604A_V23 | 90 | Blocked | Extend EZE runtime | 42 | 1162 | 0 | NotAttempted |
| S604_V10 | 90 | Blocked | Extend EZE runtime | 84 | 2548 | 0 | NotAttempted |
| S606_V14 | 90 | Blocked | Extend EZE runtime | 35 | 1153 | 0 | NotAttempted |
| S613A_V13 | 90 | Blocked | Extend EZE runtime | 44 | 1309 | 0 | NotAttempted |
| S613_V12 | 90 | Blocked | Extend EZE runtime | 44 | 1272 | 0 | NotAttempted |
| S614A_V42 | 90 | Blocked | Extend EZE runtime | 110 | 3184 | 0 | NotAttempted |
| S701A_V10 | 90 | Blocked | Extend EZE runtime | 20 | 674 | 0 | NotAttempted |
| S703A_V06 | 90 | Blocked | Extend EZE runtime | 17 | 382 | 0 | NotAttempted |
| S711A_V05 | 90 | Blocked | Extend EZE runtime | 33 | 601 | 0 | NotAttempted |
| S715A_V08 | 90 | Blocked | Extend EZE runtime | 23 | 562 | 0 | NotAttempted |
| S716A_V06 | 90 | Blocked | Extend EZE runtime | 28 | 593 | 0 | NotAttempted |
| S717A_V02 | 90 | Blocked | Extend EZE runtime | 11 | 313 | 0 | NotAttempted |
| S718A_V13 | 90 | Blocked | Extend EZE runtime | 61 | 1565 | 0 | NotAttempted |
| S720A_V12 | 90 | Blocked | Extend EZE runtime | 104 | 2576 | 0 | NotAttempted |
| S723A_V03 | 90 | Blocked | Extend EZE runtime | 30 | 670 | 0 | NotAttempted |
| S724A_V08 | 90 | Blocked | Extend EZE runtime | 51 | 1460 | 0 | NotAttempted |
| S735A_V13 | 90 | Blocked | Extend EZE runtime | 62 | 1580 | 0 | NotAttempted |
| S738A_V03 | 90 | Blocked | Extend EZE runtime | 12 | 221 | 0 | NotAttempted |
| S739A_V01 | 90 | Blocked | Extend EZE runtime | 28 | 524 | 0 | NotAttempted |
| S740A_V02 | 90 | Blocked | Extend EZE runtime | 29 | 536 | 0 | NotAttempted |
| S741A_V03 | 90 | Blocked | Extend EZE runtime | 14 | 296 | 0 | NotAttempted |
| S742A_V03 | 90 | Blocked | Extend EZE runtime | 17 | 347 | 0 | NotAttempted |
| S744A_V01 | 90 | Blocked | Extend EZE runtime | 12 | 316 | 0 | NotAttempted |
| S807A_V02 | 90 | Blocked | Extend EZE runtime | 15 | 403 | 0 | NotAttempted |
| SE15A_V30 | 90 | Blocked | Extend EZE runtime | 79 | 2679 | 0 | NotAttempted |
| SE16A_V10 | 90 | Blocked | Extend EZE runtime | 43 | 1282 | 0 | NotAttempted |
| SE17A_V10 | 90 | Blocked | Extend EZE runtime | 32 | 934 | 0 | NotAttempted |
| SE25A_V09 | 90 | Blocked | Extend EZE runtime | 43 | 1291 | 0 | NotAttempted |
| SL02A_V22 | 90 | Blocked | Extend EZE runtime | 64 | 1702 | 0 | NotAttempted |
| SL05A_V04 | 90 | Blocked | Extend EZE runtime | 23 | 547 | 0 | NotAttempted |
| SL07A_V03 | 90 | Blocked | Extend EZE runtime | 28 | 435 | 0 | NotAttempted |
| SL14A_V04 | 90 | Blocked | Extend EZE runtime | 25 | 493 | 0 | NotAttempted |
| SL18A_V01 | 90 | Blocked | Extend EZE runtime | 17 | 293 | 0 | NotAttempted |
| SL19A_V06 | 90 | Blocked | Extend EZE runtime | 25 | 593 | 0 | NotAttempted |
| SL20A_V04 | 90 | Blocked | Extend EZE runtime | 41 | 709 | 0 | NotAttempted |
| SU20A_V19 | 90 | Blocked | Extend EZE runtime | 26 | 898 | 0 | NotAttempted |
| SU40A_V17 | 90 | Blocked | Extend EZE runtime | 10 | 349 | 0 | NotAttempted |
| SU50A_V17 | 90 | Blocked | Extend EZE runtime | 11 | 388 | 0 | NotAttempted |
| TE02A_V11 | 90 | Blocked | Extend EZE runtime | 16 | 564 | 0 | NotAttempted |
| TE03A_V15 | 90 | Blocked | Extend EZE runtime | 22 | 731 | 0 | NotAttempted |
| TE05A_V04 | 90 | Blocked | Extend EZE runtime | 18 | 326 | 0 | NotAttempted |
| TE10A_V32 | 90 | Blocked | Extend EZE runtime | 53 | 1620 | 0 | NotAttempted |
| TE15A_V11 | 90 | Blocked | Extend EZE runtime | 24 | 738 | 0 | NotAttempted |
| TE25A_V09 | 90 | Blocked | Extend EZE runtime | 38 | 809 | 0 | NotAttempted |
| TE28A_V11 | 90 | Blocked | Extend EZE runtime | 18 | 488 | 0 | NotAttempted |
| TE30A_V04 | 90 | Blocked | Extend EZE runtime | 16 | 383 | 0 | NotAttempted |
| TE50A_V04 | 90 | Blocked | Extend EZE runtime | 14 | 407 | 0 | NotAttempted |
| TE55A_V13 | 90 | Blocked | Extend EZE runtime | 41 | 1528 | 0 | NotAttempted |
| TM07A_V01 | 90 | Blocked | Extend EZE runtime | 32 | 656 | 0 | NotAttempted |
| TM08A_V33 | 90 | Blocked | Extend EZE runtime | 64 | 1654 | 0 | NotAttempted |
| TM09A_V13 | 90 | Blocked | Extend EZE runtime | 52 | 985 | 0 | NotAttempted |
| TM10A_V08 | 90 | Blocked | Extend EZE runtime | 54 | 630 | 0 | NotAttempted |
| TM11A_V06 | 90 | Blocked | Extend EZE runtime | 31 | 480 | 0 | NotAttempted |
| VI20A_V12 | 90 | Blocked | Extend EZE runtime | 20 | 679 | 0 | NotAttempted |
| VI40A_V06 | 90 | Blocked | Extend EZE runtime | 15 | 302 | 0 | NotAttempted |
| X111A_V05 | 90 | Blocked | Extend EZE runtime | 77 | 3729 | 0 | NotAttempted |
| X111A_V43_6_ORODJA | 90 | Blocked | Extend EZE runtime | 76 | 3278 | 0 | NotAttempted |
| ZA00A_V11 | 90 | Blocked | Extend EZE runtime | 65 | 1408 | 0 | NotAttempted |
| ZZ50A_V03 | 90 | Blocked | Extend EZE runtime | 15 | 259 | 0 | NotAttempted |
| ZZ55A_V06 | 90 | Blocked | Extend EZE runtime | 29 | 705 | 0 | NotAttempted |
| ZZ60A_V02 | 90 | Blocked | Extend EZE runtime | 41 | 1210 | 0 | NotAttempted |
| ZZ92A_V16_EUR | 90 | Blocked | Extend EZE runtime | 34 | 650 | 0 | NotAttempted |
| ZZ92A_V24 | 90 | Blocked | Extend EZE runtime | 37 | 731 | 0 | NotAttempted |
| ZZ93A_V18 | 90 | Blocked | Extend EZE runtime | 55 | 969 | 0 | NotAttempted |
| ZZ99A_V15 | 90 | Blocked | Extend EZE runtime | 97 | 1894 | 0 | NotAttempted |
| D004A_V09 | 85 | Blocked | Extend statement parser | 9 | 382 | 3,9 | NotAttempted |
| D011A_V03 | 85 | Blocked | Extend statement parser | 28 | 802 | 1,3 | NotAttempted |
| D213A_V40 | 85 | Blocked | Extend EZE runtime | 382 | 13560 | 0 | NotAttempted |
| D238A_V02 | 85 | Blocked | Extend EZE runtime | 38 | 753 | 0 | NotAttempted |
| D278A_V14 | 85 | Blocked | Extend EZE runtime | 29 | 531 | 0 | NotAttempted |
| D279A_V38 | 85 | Blocked | Extend EZE runtime | 87 | 1990 | 0 | NotAttempted |
| D283A_V06 | 85 | Blocked | Extend EZE runtime | 27 | 648 | 0 | NotAttempted |
| D290A_V05 | 85 | Blocked | Extend EZE runtime | 20 | 423 | 0 | NotAttempted |
| D292A_V30 | 85 | Blocked | Extend EZE runtime | 108 | 2786 | 0 | NotAttempted |
| D294A_V01 | 85 | Blocked | Extend EZE runtime | 19 | 327 | 0 | NotAttempted |
| D306A_V01 | 85 | Blocked | Extend EZE runtime | 12 | 251 | 0 | NotAttempted |
| D311A_V08 | 85 | Blocked | Extend EZE runtime | 21 | 481 | 0 | NotAttempted |
| D326A_V03 | 85 | Blocked | Extend EZE runtime | 19 | 461 | 0 | NotAttempted |
| D334A_V09 | 85 | Blocked | Extend EZE runtime | 23 | 469 | 0 | NotAttempted |
| KK03A_V06 | 85 | Blocked | Extend EZE runtime | 21 | 647 | 0 | NotAttempted |
| MENIJI_09_11_2006 | 85 | Blocked | Extend EZE runtime | 227 | 15303 | 0 | NotAttempted |
| NA20A_V66 | 85 | Blocked | Extend EZE runtime | 100 | 3287 | 0 | NotAttempted |
| PZ22A_V04 | 85 | Blocked | Extend EZE runtime | 18 | 267 | 0 | NotAttempted |
| S213A_V26_EUR | 85 | Blocked | Extend EZE runtime | 30 | 1097 | 0 | NotAttempted |
| S213A_V27_EUR | 85 | Blocked | Extend EZE runtime | 30 | 1136 | 0 | NotAttempted |
| S213A_V28_EUR | 85 | Blocked | Extend EZE runtime | 30 | 1134 | 0 | NotAttempted |
| S213A_V33 | 85 | Blocked | Extend EZE runtime | 32 | 1170 | 0 | NotAttempted |
| S743A_V05 | 85 | Blocked | Extend EZE runtime | 29 | 609 | 0 | NotAttempted |
| S800A_V51 | 85 | Blocked | Extend EZE runtime | 362 | 14003 | 0 | NotAttempted |
| S801A_V04A | 85 | Blocked | Extend EZE runtime | 84 | 2094 | 0 | NotAttempted |
| S801A_V08 | 85 | Blocked | Extend EZE runtime | 84 | 2130 | 0 | NotAttempted |
| SL09A_V18 | 85 | Blocked | Extend EZE runtime | 58 | 1458 | 0 | NotAttempted |
| SL16A_V03 | 85 | Blocked | Extend EZE runtime | 21 | 430 | 0 | NotAttempted |
| TE32A_V06 | 85 | Blocked | Extend EZE runtime | 15 | 387 | 0 | NotAttempted |
| TE35A_V10 | 85 | Blocked | Extend EZE runtime | 41 | 2228 | 0 | NotAttempted |
| TE63A_V02 | 85 | Blocked | Extend EZE runtime | 30 | 840 | 0 | NotAttempted |
| CE03A_V07 | 80 | Blocked | Extend EZE runtime | 27 | 610 | 0 | NotAttempted |
| PR12A_V04 | 80 | Blocked | Extend EZE runtime | 21 | 426 | 0 | NotAttempted |
| S524A_V02 | 80 | Blocked | Extend EZE runtime | 9 | 126 | 0 | NotAttempted |
| SA47A_V02 | 80 | Blocked | Extend EZE runtime | 9 | 126 | 0 | NotAttempted |
| SU14A_02_NTESF | 80 | Blocked | Extend EZE runtime | 29 | 553 | 0 | NotAttempted |
| SU14A_V03 | 80 | Blocked | Extend EZE runtime | 29 | 553 | 0 | NotAttempted |
| CT25A_V39 | 75 | High | Extend EZE runtime | 71 | 2323 | 0 | NotAttempted |
| CT28A_V28 | 75 | High | Extend EZE runtime | 23 | 605 | 0 | NotAttempted |
| CT32A_V05 | 75 | High | Extend EZE runtime | 42 | 814 | 0 | NotAttempted |
| D001A_V13 | 75 | High | Extend EZE runtime | 14 | 422 | 0 | NotAttempted |
| D010A_V05 | 75 | High | Extend EZE runtime | 24 | 738 | 0 | NotAttempted |
| D101A_V19 | 75 | High | Extend EZE runtime | 64 | 1734 | 0 | NotAttempted |
| D126A_V36 | 75 | High | Extend EZE runtime | 77 | 2279 | 0 | NotAttempted |
| D130A_V19 | 75 | High | Extend EZE runtime | 70 | 1674 | 0 | NotAttempted |
| D141A_V17 | 75 | High | Extend EZE runtime | 85 | 2488 | 0 | NotAttempted |
| D141A_V17_OBV | 75 | High | Extend EZE runtime | 71 | 2234 | 0 | NotAttempted |
| D143A_V03 | 75 | High | Extend EZE runtime | 44 | 2259 | 0 | NotAttempted |
| D153A_V13 | 75 | High | Extend EZE runtime | 34 | 1202 | 0 | NotAttempted |
| D194A_V30 | 75 | High | Extend EZE runtime | 50 | 1801 | 0 | NotAttempted |
| D196A_V14 | 75 | High | Extend EZE runtime | 31 | 1013 | 0 | NotAttempted |
| D197A_V12 | 75 | High | Extend EZE runtime | 39 | 1107 | 0 | NotAttempted |
| D209A_V04 | 75 | High | Extend EZE runtime | 46 | 1076 | 0 | NotAttempted |
| D214A_V10 | 75 | High | Extend EZE runtime | 26 | 1669 | 0 | NotAttempted |
| D218A_V05 | 75 | High | Extend EZE runtime | 30 | 815 | 0 | NotAttempted |
| D233A_V09 | 75 | High | Extend EZE runtime | 24 | 779 | 0 | NotAttempted |
| D241A_V16 | 75 | High | Extend EZE runtime | 44 | 759 | 0 | NotAttempted |
| D255A_V01 | 75 | High | Extend EZE runtime | 20 | 436 | 0 | NotAttempted |
| D261A_V02 | 75 | High | Extend EZE runtime | 16 | 295 | 0 | NotAttempted |
| D265A_V01 | 75 | High | Extend EZE runtime | 15 | 258 | 0 | NotAttempted |
| D267A_V02 | 75 | High | Extend EZE runtime | 12 | 221 | 0 | NotAttempted |
| D280A_V01 | 75 | High | Extend EZE runtime | 20 | 295 | 0 | NotAttempted |
| D281A_V15 | 75 | High | Extend EZE runtime | 48 | 1266 | 0 | NotAttempted |
| D284A_V08 | 75 | High | Extend EZE runtime | 35 | 678 | 0 | NotAttempted |
| D295A_V01 | 75 | High | Extend EZE runtime | 9 | 152 | 0 | NotAttempted |
| D303A_V08 | 75 | High | Extend EZE runtime | 29 | 580 | 0 | NotAttempted |
| D310A_V06 | 75 | High | Extend EZE runtime | 23 | 393 | 0 | NotAttempted |
| D314A_V06 | 75 | High | Extend EZE runtime | 71 | 1428 | 0 | NotAttempted |
| D333A_V04 | 75 | High | Extend EZE runtime | 19 | 408 | 0 | NotAttempted |
| D344_V04 | 75 | High | Extend EZE runtime | 38 | 787 | 0 | NotAttempted |
| DL15A_V11 | 75 | High | Extend EZE runtime | 27 | 637 | 0 | NotAttempted |
| DL16A_V15 | 75 | High | Extend EZE runtime | 15 | 440 | 0 | NotAttempted |
| DL21A_V36A | 75 | High | Extend EZE runtime | 37 | 1665 | 0 | NotAttempted |
| DL21A_V37 | 75 | High | Extend EZE runtime | 35 | 1593 | 0 | NotAttempted |
| DL22A_V15 | 75 | High | Extend EZE runtime | 30 | 980 | 0 | NotAttempted |
| DL23A_V23 | 75 | High | Extend EZE runtime | 38 | 1255 | 0 | NotAttempted |
| DL24A_V25 | 75 | High | Extend EZE runtime | 43 | 1126 | 0 | NotAttempted |
| DL28A_V17 | 75 | High | Extend EZE runtime | 24 | 598 | 0 | NotAttempted |
| DL29A_V12 | 75 | High | Extend EZE runtime | 20 | 560 | 0 | NotAttempted |
| DL37A_V25 | 75 | High | Extend EZE runtime | 50 | 1562 | 0 | NotAttempted |
| DL41A_V05 | 75 | High | Extend EZE runtime | 24 | 505 | 0 | NotAttempted |
| DL44A_V43 | 75 | High | Extend EZE runtime | 96 | 2534 | 0 | NotAttempted |
| DL45A_V12 | 75 | High | Extend EZE runtime | 45 | 1066 | 0 | NotAttempted |
| DL46A_V10 | 75 | High | Extend EZE runtime | 38 | 962 | 0 | NotAttempted |
| DL47A_V59 | 75 | High | Extend EZE runtime | 61 | 1672 | 0 | NotAttempted |
| DL48A_V06 | 75 | High | Extend EZE runtime | 59 | 1764 | 0 | NotAttempted |
| DL49A_V02 | 75 | High | Extend EZE runtime | 40 | 946 | 0 | NotAttempted |
| DL51A_V12 | 75 | High | Extend EZE runtime | 35 | 1732 | 0 | NotAttempted |
| DL52A_V10 | 75 | High | Extend EZE runtime | 36 | 1760 | 0 | NotAttempted |
| DL55A_V11 | 75 | High | Extend EZE runtime | 27 | 1044 | 0 | NotAttempted |
| DL60A_V17 | 75 | High | Extend EZE runtime | 20 | 963 | 0 | NotAttempted |
| DL63A_V07 | 75 | High | Extend EZE runtime | 22 | 398 | 0 | NotAttempted |
| DL70A_V08 | 75 | High | Extend EZE runtime | 25 | 668 | 0 | NotAttempted |
| DL80A_V09 | 75 | High | Extend EZE runtime | 20 | 530 | 0 | NotAttempted |
| DL95A_V11 | 75 | High | Extend EZE runtime | 26 | 635 | 0 | NotAttempted |
| DL96A_V03 | 75 | High | Extend EZE runtime | 24 | 539 | 0 | NotAttempted |
| IMP0A_V11 | 75 | High | Extend EZE runtime | 14 | 335 | 0 | NotAttempted |
| IS02A_V12_HOLDING | 75 | High | Extend EZE runtime | 12 | 285 | 0 | NotAttempted |
| KA10A_V11 | 75 | High | Extend EZE runtime | 22 | 793 | 0 | NotAttempted |
| KA10A_V12_EUR | 75 | High | Extend EZE runtime | 20 | 784 | 0 | NotAttempted |
| KA14A_V06 | 75 | High | Extend EZE runtime | 8 | 165 | 0 | NotAttempted |
| KA61A_V04 | 75 | High | Extend EZE runtime | 18 | 773 | 0 | NotAttempted |
| LS14A_VXX | 75 | High | Extend EZE runtime | 15 | 413 | 0 | NotAttempted |
| M000A_V16A | 75 | High | Extend EZE runtime | 7 | 428 | 0 | NotAttempted |
| M000A_V22 | 75 | High | Extend EZE runtime | 7 | 450 | 0 | NotAttempted |
| M012A_V18 | 75 | High | Extend EZE runtime | 5 | 265 | 0 | NotAttempted |
| M022A_V06 | 75 | High | Extend EZE runtime | 5 | 161 | 0 | NotAttempted |
| M074A_V11 | 75 | High | Extend EZE runtime | 16 | 371 | 0 | NotAttempted |
| ME00A_V32_HOLDING | 75 | High | Extend EZE runtime | 8 | 346 | 0 | NotAttempted |
| ME00A_V33 | 75 | High | Extend EZE runtime | 8 | 352 | 0 | NotAttempted |
| ME22A_V07 | 75 | High | Extend EZE runtime | 5 | 161 | 0 | NotAttempted |
| ME34A_V02 | 75 | High | Extend EZE runtime | 2 | 94 | 0 | NotAttempted |
| MI35A_V22 | 75 | High | Extend EZE runtime | 37 | 528 | 0 | NotAttempted |
| NA10A_V43 | 75 | High | Extend EZE runtime | 79 | 2128 | 0 | NotAttempted |
| NA30A_V74 | 75 | High | Extend EZE runtime | 42 | 2252 | 0 | NotAttempted |
| NA35A_V39 | 75 | High | Extend EZE runtime | 39 | 1806 | 0 | NotAttempted |
| NA51A_V27 | 75 | High | Extend EZE runtime | 21 | 660 | 0 | NotAttempted |
| NA52A_V40 | 75 | High | Extend EZE runtime | 35 | 1246 | 0 | NotAttempted |
| NA53A_V23 | 75 | High | Extend EZE runtime | 28 | 754 | 0 | NotAttempted |
| NA54A_V38 | 75 | High | Extend EZE runtime | 45 | 1391 | 0 | NotAttempted |
| NA57A_V44 | 75 | High | Extend EZE runtime | 55 | 1161 | 0 | NotAttempted |
| NA59A_V24 | 75 | High | Extend EZE runtime | 38 | 773 | 0 | NotAttempted |
| NA63A_V23 | 75 | High | Extend EZE runtime | 40 | 946 | 0 | NotAttempted |
| NA76A_V08 | 75 | High | Extend EZE runtime | 25 | 894 | 0 | NotAttempted |
| NA77A_V07 | 75 | High | Extend EZE runtime | 34 | 999 | 0 | NotAttempted |
| PR11A_V03 | 75 | High | Extend EZE runtime | 15 | 653 | 0 | NotAttempted |
| PR41A_V26 | 75 | High | Extend EZE runtime | 33 | 736 | 0 | NotAttempted |
| PR46A_V19 | 75 | High | Extend EZE runtime | 32 | 926 | 0 | NotAttempted |
| PR47A_V14 | 75 | High | Extend EZE runtime | 18 | 508 | 0 | NotAttempted |
| PR48A_V16 | 75 | High | Extend EZE runtime | 27 | 497 | 0 | NotAttempted |
| PR63A_V03 | 75 | High | Extend EZE runtime | 17 | 260 | 0 | NotAttempted |
| RE07A_V20 | 75 | High | Extend EZE runtime | 49 | 1048 | 0 | NotAttempted |
| RE37A_V14 | 75 | High | Extend EZE runtime | 47 | 1136 | 0 | NotAttempted |
| RE87A_V25 | 75 | High | Extend EZE runtime | 61 | 1576 | 0 | NotAttempted |
| RO04A_V11 | 75 | High | Extend EZE runtime | 12 | 615 | 0 | NotAttempted |
| RO22A_V38 | 75 | High | Extend EZE runtime | 27 | 697 | 0 | NotAttempted |
| RO25A_V06 | 75 | High | Extend EZE runtime | 11 | 244 | 0 | NotAttempted |
| RO26A_V07 | 75 | High | Extend EZE runtime | 14 | 320 | 0 | NotAttempted |
| RO31A_V17 | 75 | High | Extend EZE runtime | 36 | 1039 | 0 | NotAttempted |
| RO40A_V24 | 75 | High | Extend EZE runtime | 20 | 448 | 0 | NotAttempted |
| S003A_V05 | 75 | High | Extend EZE runtime | 12 | 272 | 0 | NotAttempted |
| S007A_V07 | 75 | High | Extend EZE runtime | 12 | 281 | 0 | NotAttempted |
| S009A_V07 | 75 | High | Extend EZE runtime | 23 | 521 | 0 | NotAttempted |
| S100A_V05 | 75 | High | Extend EZE runtime | 12 | 321 | 0 | NotAttempted |
| S112A_V10 | 75 | High | Extend EZE runtime | 40 | 1137 | 0 | NotAttempted |
| S117A_V04 | 75 | High | Extend EZE runtime | 8 | 241 | 0 | NotAttempted |
| S201A_V07 | 75 | High | Extend EZE runtime | 28 | 895 | 0 | NotAttempted |
| S205A_V10_ESF | 75 | High | Extend EZE runtime | 58 | 1253 | 0 | NotAttempted |
| S205A_V10_EURZ | 75 | High | Extend EZE runtime | 58 | 1253 | 0 | NotAttempted |
| S205A_V12 | 75 | High | Extend EZE runtime | 56 | 1240 | 0 | NotAttempted |
| S207A_V09 | 75 | High | Extend EZE runtime | 13 | 414 | 0 | NotAttempted |
| S401A_V03 | 75 | High | Extend EZE runtime | 84 | 1492 | 0 | NotAttempted |
| S506A_V25 | 75 | High | Extend EZE runtime | 16 | 649 | 0 | NotAttempted |
| S511A_V04 | 75 | High | Extend EZE runtime | 20 | 650 | 0 | NotAttempted |
| S512A_V16 | 75 | High | Extend EZE runtime | 29 | 921 | 0 | NotAttempted |
| S514A_V18_HOLDING | 75 | High | Extend EZE runtime | 7 | 285 | 0 | NotAttempted |
| S514A_V21 | 75 | High | Extend EZE runtime | 7 | 286 | 0 | NotAttempted |
| S517A_V10 | 75 | High | Extend EZE runtime | 44 | 1554 | 0 | NotAttempted |
| S522A_V12 | 75 | High | Extend EZE runtime | 23 | 718 | 0 | NotAttempted |
| S525A_V18 | 75 | High | Extend EZE runtime | 45 | 1281 | 0 | NotAttempted |
| S526A_V11 | 75 | High | Extend EZE runtime | 42 | 1420 | 0 | NotAttempted |
| S527A_V09 | 75 | High | Extend EZE runtime | 55 | 2265 | 0 | NotAttempted |
| S534A_V02 | 75 | High | Extend EZE runtime | 20 | 470 | 0 | NotAttempted |
| S537A_V02 | 75 | High | Extend EZE runtime | 17 | 408 | 0 | NotAttempted |
| S548A_V05 | 75 | High | Extend EZE runtime | 21 | 515 | 0 | NotAttempted |
| S580_V06 | 75 | High | Extend EZE runtime | 32 | 694 | 0 | NotAttempted |
| S581_V09 | 75 | High | Extend EZE runtime | 49 | 1236 | 0 | NotAttempted |
| S584_V04 | 75 | High | Extend EZE runtime | 48 | 1347 | 0 | NotAttempted |
| S593_V05 | 75 | High | Extend EZE runtime | 22 | 504 | 0 | NotAttempted |
| S596_V03 | 75 | High | Extend EZE runtime | 25 | 583 | 0 | NotAttempted |
| S597A_V05 | 75 | High | Extend EZE runtime | 14 | 310 | 0 | NotAttempted |
| S597_V03 | 75 | High | Extend EZE runtime | 14 | 304 | 0 | NotAttempted |
| S611_V04 | 75 | High | Extend EZE runtime | 14 | 380 | 0 | NotAttempted |
| S616_V08 | 75 | High | Extend EZE runtime | 31 | 687 | 0 | NotAttempted |
| S719A_V04 | 75 | High | Extend EZE runtime | 45 | 1016 | 0 | NotAttempted |
| S731A_V18 | 75 | High | Extend EZE runtime | 66 | 2177 | 0 | NotAttempted |
| S732A_V18A | 75 | High | Extend EZE runtime | 95 | 3644 | 0 | NotAttempted |
| S732A_V19 | 75 | High | Extend EZE runtime | 96 | 3664 | 0 | NotAttempted |
| S733A_V16 | 75 | High | Extend EZE runtime | 60 | 1865 | 0 | NotAttempted |
| S734A_V10 | 75 | High | Extend EZE runtime | 43 | 683 | 0 | NotAttempted |
| S802A_V09 | 75 | High | Extend EZE runtime | 49 | 1560 | 0 | NotAttempted |
| S802_V01 | 75 | High | Extend EZE runtime | 46 | 1198 | 0 | NotAttempted |
| S806A_V05 | 75 | High | Extend EZE runtime | 33 | 768 | 0 | NotAttempted |
| SA10A_V28_NT | 75 | High | Extend EZE runtime | 114 | 4140 | 0 | NotAttempted |
| SA10A_V31 | 75 | High | Extend EZE runtime | 24 | 1123 | 0 | NotAttempted |
| SA13A_V18 | 75 | High | Extend EZE runtime | 17 | 381 | 0 | NotAttempted |
| SE01A_V09 | 75 | High | Extend EZE runtime | 9 | 219 | 0 | NotAttempted |
| SE02A_V02 | 75 | High | Extend EZE runtime | 21 | 477 | 0 | NotAttempted |
| SE07A_03 | 75 | High | Extend EZE runtime | 14 | 526 | 0 | NotAttempted |
| SE07A_04 | 75 | High | Extend EZE runtime | 14 | 526 | 0 | NotAttempted |
| SE08A_V20 | 75 | High | Extend EZE runtime | 43 | 1481 | 0 | NotAttempted |
| SE09A_V06 | 75 | High | Extend EZE runtime | 68 | 2247 | 0 | NotAttempted |
| SE10A_V04 | 75 | High | Extend EZE runtime | 23 | 770 | 0 | NotAttempted |
| SE11A_V12 | 75 | High | Extend EZE runtime | 16 | 511 | 0 | NotAttempted |
| SE12A_V46 | 75 | High | Extend EZE runtime | 76 | 2772 | 0 | NotAttempted |
| SE13A_V57 | 75 | High | Extend EZE runtime | 97 | 3871 | 0 | NotAttempted |
| SE18A_V18 | 75 | High | Extend EZE runtime | 37 | 908 | 0 | NotAttempted |
| SE23A_V11 | 75 | High | Extend EZE runtime | 28 | 671 | 0 | NotAttempted |
| SE29A_V14 | 75 | High | Extend EZE runtime | 44 | 1061 | 0 | NotAttempted |
| SE32A_V03 | 75 | High | Extend EZE runtime | 21 | 500 | 0 | NotAttempted |
| SE34A_V17 | 75 | High | Extend EZE runtime | 44 | 768 | 0 | NotAttempted |
| SL04A_V13 | 75 | High | Extend EZE runtime | 54 | 720 | 0 | NotAttempted |
| SL06A_V07 | 75 | High | Extend EZE runtime | 32 | 459 | 0 | NotAttempted |
| SL11A_V03 | 75 | High | Extend EZE runtime | 18 | 463 | 0 | NotAttempted |
| SL12A_V03 | 75 | High | Extend EZE runtime | 14 | 276 | 0 | NotAttempted |
| SU30A_V23Z | 75 | High | Extend EZE runtime | 35 | 801 | 0 | NotAttempted |
| SU30A_V24Z | 75 | High | Extend EZE runtime | 35 | 802 | 0 | NotAttempted |
| SU30A_V28 | 75 | High | Extend EZE runtime | 37 | 860 | 0 | NotAttempted |
| TE01A_V09 | 75 | High | Extend EZE runtime | 10 | 174 | 0 | NotAttempted |
| TE22A_V16 | 75 | High | Extend EZE runtime | 11 | 323 | 0 | NotAttempted |
| TE26A_V06 | 75 | High | Extend EZE runtime | 13 | 311 | 0 | NotAttempted |
| TE31A_V07 | 75 | High | Extend EZE runtime | 20 | 707 | 0 | NotAttempted |
| TE33A_V04 | 75 | High | Extend EZE runtime | 44 | 2259 | 0 | NotAttempted |
| TE37A_V07 | 75 | High | Extend EZE runtime | 31 | 758 | 0 | NotAttempted |
| TE41A_V07 | 75 | High | Extend EZE runtime | 10 | 317 | 0 | NotAttempted |
| TE42A_V12 | 75 | High | Extend EZE runtime | 6 | 253 | 0 | NotAttempted |
| TE62A_V02 | 75 | High | Extend EZE runtime | 29 | 969 | 0 | NotAttempted |
| TE70A_V02 | 75 | High | Extend EZE runtime | 24 | 681 | 0 | NotAttempted |
| TM03A_V17 | 75 | High | Extend EZE runtime | 35 | 1018 | 0 | NotAttempted |
| VI10A_V13 | 75 | High | Extend EZE runtime | 24 | 863 | 0 | NotAttempted |
| VI11A_V06 | 75 | High | Extend EZE runtime | 22 | 727 | 0 | NotAttempted |
| VI13A_V04 | 75 | High | Extend EZE runtime | 16 | 325 | 0 | NotAttempted |
| VI15A_V07 | 75 | High | Extend EZE runtime | 39 | 821 | 0 | NotAttempted |
| VI16A_V04 | 75 | High | Extend EZE runtime | 10 | 231 | 0 | NotAttempted |
| VI17A_V04 | 75 | High | Extend EZE runtime | 9 | 218 | 0 | NotAttempted |
| VI21A_V07 | 75 | High | Extend EZE runtime | 20 | 494 | 0 | NotAttempted |
| VI45A_V07 | 75 | High | Extend EZE runtime | 21 | 445 | 0 | NotAttempted |
| ZZ71A_V07 | 75 | High | Extend EZE runtime | 15 | 393 | 0 | NotAttempted |
| ZZ72A_V03 | 75 | High | Extend EZE runtime | 11 | 290 | 0 | NotAttempted |
| ZZ73A_V04 | 75 | High | Extend EZE runtime | 11 | 264 | 0 | NotAttempted |
| ZZ74A_V06 | 75 | High | Extend EZE runtime | 19 | 376 | 0 | NotAttempted |
| ZZ81A_V11 | 75 | High | Extend EZE runtime | 18 | 423 | 0 | NotAttempted |
| ZZ82A_V06 | 75 | High | Extend EZE runtime | 11 | 258 | 0 | NotAttempted |
| ZZ83A_V08 | 75 | High | Extend EZE runtime | 11 | 286 | 0 | NotAttempted |
| ZZ84A_V04 | 75 | High | Extend EZE runtime | 34 | 790 | 0 | NotAttempted |
| ZZ86A_V04 | 75 | High | Extend EZE runtime | 31 | 453 | 0 | NotAttempted |
| ZZ91A_V26 | 75 | High | Extend EZE runtime | 39 | 699 | 0 | NotAttempted |
| ZZ94A_V12 | 75 | High | Extend EZE runtime | 30 | 634 | 0 | NotAttempted |
| ZZ95A_V06 | 75 | High | Extend EZE runtime | 75 | 1090 | 0 | NotAttempted |
| D173A_V11 | 70 | High | UI/map runtime needed | 41 | 1766 | 0 | NotAttempted |
| IN72A_V74 | 70 | High | UI/map runtime needed | 209 | 5814 | 0 | NotAttempted |
| ME91A_V05 | 70 | High | Extend EZE runtime | 3 | 42 | 0 | NotAttempted |
| NA44A_V50 | 70 | High | UI/map runtime needed | 24 | 718 | 0 | NotAttempted |
| S001A_V04 | 70 | High | UI/map runtime needed | 91 | 1983 | 0 | NotAttempted |
| S010A_V02 | 70 | High | UI/map runtime needed | 92 | 1784 | 0 | NotAttempted |
| S050A_V09 | 70 | High | UI/map runtime needed | 65 | 1924 | 0 | NotAttempted |
| S052A_V03 | 70 | High | UI/map runtime needed | 108 | 2936 | 0 | NotAttempted |
| S053A_V02 | 70 | High | UI/map runtime needed | 130 | 2755 | 0 | NotAttempted |
| S115A_V04 | 70 | High | UI/map runtime needed | 36 | 1024 | 0 | NotAttempted |
| S206A_V06 | 70 | High | UI/map runtime needed | 30 | 758 | 0 | NotAttempted |
| S208A_V08 | 70 | High | UI/map runtime needed | 20 | 413 | 0 | NotAttempted |
| S217A_V03 | 70 | High | UI/map runtime needed | 84 | 1848 | 0 | NotAttempted |
| S403A_V06 | 70 | High | UI/map runtime needed | 40 | 744 | 0 | NotAttempted |
| S722A_V01 | 70 | High | UI/map runtime needed | 7 | 141 | 0 | NotAttempted |
| TM06A_V04 | 70 | High | UI/map runtime needed | 10 | 224 | 0 | NotAttempted |
| D327A_V02 | 65 | High | Extend EZE runtime | 26 | 611 | 0 | NotAttempted |
| M027A_V02 | 65 | High | UI/map runtime needed | 4 | 81 | 0 | NotAttempted |
| ME27A_V02 | 65 | High | UI/map runtime needed | 4 | 81 | 0 | NotAttempted |
| ME90A_V02 | 65 | High | UI/map runtime needed | 5 | 125 | 0 | NotAttempted |
| MI10A_V49B | 65 | High | UI/map runtime needed | 107 | 2758 | 0 | NotAttempted |
| MI10A_V49C | 65 | High | UI/map runtime needed | 107 | 2759 | 0 | NotAttempted |
| S301A_V10 | 65 | High | UI/map runtime needed | 58 | 1085 | 0 | NotAttempted |
| S303A_V03 | 65 | High | UI/map runtime needed | 13 | 239 | 0 | NotAttempted |
| CT16A_V26 | 60 | High | Extend EZE runtime | 228 | 11336 | 0 | NotAttempted |
| CT27A_V90 | 60 | High | Extend EZE runtime | 105 | 5318 | 0 | NotAttempted |
| D124A_V16 | 60 | High | Extend EZE runtime | 37 | 861 | 0 | NotAttempted |
| D308A_V09 | 60 | High | Extend EZE runtime | 39 | 867 | 0 | NotAttempted |
| D332A_V05 | 60 | High | Extend EZE runtime | 78 | 1465 | 0 | NotAttempted |
| D337A_V07 | 60 | High | Extend EZE runtime | 104 | 3150 | 0 | NotAttempted |
| KA21A_V09 | 60 | High | Extend EZE runtime | 14 | 547 | 0 | NotAttempted |
| KA33A_V06 | 60 | High | Extend EZE runtime | 17 | 726 | 0 | NotAttempted |
| KA33A_V07_EUR | 60 | High | Extend EZE runtime | 19 | 758 | 0 | NotAttempted |
| TU10A_V02 | 60 | High | Extend EZE runtime | 12 | 177 | 0 | NotAttempted |
| BO01A_V04_HOLDING | 55 | High | UI/map runtime needed | 3 | 64 | 0 | NotAttempted |
| CE01A_V07 | 55 | High | UI/map runtime needed | 8 | 371 | 0 | NotAttempted |
| D145A_V09 | 55 | High | UI/map runtime needed | 45 | 1982 | 0 | NotAttempted |
| F001A_V06_HOLDING | 55 | High | UI/map runtime needed | 5 | 201 | 0 | NotAttempted |
| IMPOA_V15_HOLDING | 55 | High | UI/map runtime needed | 5 | 216 | 0 | NotAttempted |
| IMPOA_V18 | 55 | High | UI/map runtime needed | 6 | 225 | 0 | NotAttempted |
| IS01A_V12_HOLDING | 55 | High | UI/map runtime needed | 3 | 113 | 0 | NotAttempted |
| IS03A_V12_HOLDING | 55 | High | UI/map runtime needed | 3 | 130 | 0 | NotAttempted |
| IS03A_V13 | 55 | High | UI/map runtime needed | 3 | 140 | 0 | NotAttempted |
| LS01A_V14 | 55 | High | UI/map runtime needed | 2 | 224 | 0 | NotAttempted |
| LS22A_V07 | 55 | High | UI/map runtime needed | 2 | 118 | 0 | NotAttempted |
| M001A_V53 | 55 | High | UI/map runtime needed | 8 | 712 | 0 | NotAttempted |
| M002A_V38 | 55 | High | UI/map runtime needed | 6 | 374 | 0 | NotAttempted |
| M003A_V21 | 55 | High | UI/map runtime needed | 5 | 388 | 0 | NotAttempted |
| M004 | 55 | High | UI/map runtime needed | 3 | 351 | 0 | NotAttempted |
| M004A_V22 | 55 | High | UI/map runtime needed | 3 | 351 | 0 | NotAttempted |
| M005A_V15 | 55 | High | UI/map runtime needed | 3 | 249 | 0 | NotAttempted |
| M006A_V03 | 55 | High | UI/map runtime needed | 3 | 154 | 0 | NotAttempted |
| M007A_V08 | 55 | High | UI/map runtime needed | 3 | 148 | 0 | NotAttempted |
| M008A_V06 | 55 | High | UI/map runtime needed | 3 | 104 | 0 | NotAttempted |
| M009A_V49 | 55 | High | UI/map runtime needed | 10 | 699 | 0 | NotAttempted |
| M010A_V15 | 55 | High | UI/map runtime needed | 5 | 259 | 0 | NotAttempted |
| M011A_V04 | 55 | High | UI/map runtime needed | 3 | 107 | 0 | NotAttempted |
| M013A_V07 | 55 | High | UI/map runtime needed | 3 | 106 | 0 | NotAttempted |
| M014A_V02 | 55 | High | UI/map runtime needed | 3 | 93 | 0 | NotAttempted |
| M015A_V18 | 55 | High | UI/map runtime needed | 3 | 297 | 0 | NotAttempted |
| M016A_V15 | 55 | High | UI/map runtime needed | 3 | 304 | 0 | NotAttempted |
| M017A_V06 | 55 | High | UI/map runtime needed | 3 | 101 | 0 | NotAttempted |
| M018A_V13 | 55 | High | UI/map runtime needed | 6 | 783 | 0 | NotAttempted |
| M019A_V03 | 55 | High | UI/map runtime needed | 3 | 76 | 0 | NotAttempted |
| M020A_V04 | 55 | High | UI/map runtime needed | 3 | 77 | 0 | NotAttempted |
| M021A_V05 | 55 | High | UI/map runtime needed | 3 | 94 | 0 | NotAttempted |
| M023A_V16 | 55 | High | UI/map runtime needed | 3 | 486 | 0 | NotAttempted |
| M024A_V16 | 55 | High | UI/map runtime needed | 3 | 244 | 0 | NotAttempted |
| M025A_V04 | 55 | High | UI/map runtime needed | 3 | 96 | 0 | NotAttempted |
| M028A_V10 | 55 | High | UI/map runtime needed | 3 | 127 | 0 | NotAttempted |
| M030A_V04 | 55 | High | UI/map runtime needed | 3 | 102 | 0 | NotAttempted |
| M031A_V06 | 55 | High | UI/map runtime needed | 7 | 553 | 0 | NotAttempted |
| M033A_V12 | 55 | High | UI/map runtime needed | 3 | 230 | 0 | NotAttempted |
| M040A_V54 | 55 | High | UI/map runtime needed | 10 | 869 | 0 | NotAttempted |
| M041A_V07 | 55 | High | UI/map runtime needed | 3 | 195 | 0 | NotAttempted |
| M042A_V16 | 55 | High | UI/map runtime needed | 3 | 319 | 0 | NotAttempted |
| M043A_V10 | 55 | High | UI/map runtime needed | 3 | 208 | 0 | NotAttempted |
| M044A_V02 | 55 | High | UI/map runtime needed | 3 | 146 | 0 | NotAttempted |
| M045A_V03 | 55 | High | UI/map runtime needed | 3 | 129 | 0 | NotAttempted |
| M049A_V10 | 55 | High | UI/map runtime needed | 3 | 148 | 0 | NotAttempted |
| M050A_V05 | 55 | High | UI/map runtime needed | 3 | 113 | 0 | NotAttempted |
| M051A_V10 | 55 | High | UI/map runtime needed | 3 | 432 | 0 | NotAttempted |
| M052A_V17 | 55 | High | UI/map runtime needed | 5 | 481 | 0 | NotAttempted |
| M053A_V09 | 55 | High | UI/map runtime needed | 3 | 193 | 0 | NotAttempted |
| M054A_V06 | 55 | High | UI/map runtime needed | 3 | 168 | 0 | NotAttempted |
| M055A_V11 | 55 | High | UI/map runtime needed | 3 | 277 | 0 | NotAttempted |
| M056A_V02 | 55 | High | UI/map runtime needed | 3 | 138 | 0 | NotAttempted |
| M060A_V02 | 55 | High | UI/map runtime needed | 4 | 98 | 0 | NotAttempted |
| M061A_V15 | 55 | High | UI/map runtime needed | 3 | 438 | 0 | NotAttempted |
| M062A_V02 | 55 | High | UI/map runtime needed | 3 | 253 | 0 | NotAttempted |
| M063A_V04 | 55 | High | UI/map runtime needed | 3 | 187 | 0 | NotAttempted |
| M064A_V04 | 55 | High | UI/map runtime needed | 6 | 401 | 0 | NotAttempted |
| M065A_V01 | 55 | High | UI/map runtime needed | 2 | 122 | 0 | NotAttempted |
| M070A_V10 | 55 | High | UI/map runtime needed | 3 | 110 | 0 | NotAttempted |
| M071A_V11 | 55 | High | UI/map runtime needed | 3 | 142 | 0 | NotAttempted |
| M072A_V10 | 55 | High | UI/map runtime needed | 3 | 125 | 0 | NotAttempted |
| M073A_V35 | 55 | High | UI/map runtime needed | 7 | 514 | 0 | NotAttempted |
| M075A_V05 | 55 | High | UI/map runtime needed | 4 | 126 | 0 | NotAttempted |
| M076A_V09 | 55 | High | UI/map runtime needed | 7 | 320 | 0 | NotAttempted |
| M077A_V03 | 55 | High | UI/map runtime needed | 3 | 165 | 0 | NotAttempted |
| M078 | 55 | High | UI/map runtime needed | 3 | 103 | 0 | NotAttempted |
| M078A_V02 | 55 | High | UI/map runtime needed | 3 | 103 | 0 | NotAttempted |
| M079A_V05 | 55 | High | UI/map runtime needed | 3 | 128 | 0 | NotAttempted |
| M080A_V18 | 55 | High | UI/map runtime needed | 3 | 307 | 0 | NotAttempted |
| M081A_V30 | 55 | High | UI/map runtime needed | 3 | 288 | 0 | NotAttempted |
| M082A_V05 | 55 | High | UI/map runtime needed | 3 | 136 | 0 | NotAttempted |
| M083A_V38 | 55 | High | UI/map runtime needed | 8 | 532 | 0 | NotAttempted |
| M084A_V20 | 55 | High | UI/map runtime needed | 3 | 233 | 0 | NotAttempted |
| M085A_V18 | 55 | High | UI/map runtime needed | 3 | 211 | 0 | NotAttempted |
| M086A_V03 | 55 | High | UI/map runtime needed | 3 | 143 | 0 | NotAttempted |
| M087A_V08 | 55 | High | UI/map runtime needed | 3 | 173 | 0 | NotAttempted |
| M088A_V16 | 55 | High | UI/map runtime needed | 3 | 277 | 0 | NotAttempted |
| M089A_V10 | 55 | High | UI/map runtime needed | 3 | 152 | 0 | NotAttempted |
| M090A_V04 | 55 | High | UI/map runtime needed | 3 | 214 | 0 | NotAttempted |
| M091A_V02 | 55 | High | UI/map runtime needed | 3 | 155 | 0 | NotAttempted |
| M092A_V05 | 55 | High | UI/map runtime needed | 3 | 174 | 0 | NotAttempted |
| M093A_V03 | 55 | High | UI/map runtime needed | 3 | 149 | 0 | NotAttempted |
| M094A_V02 | 55 | High | UI/map runtime needed | 3 | 173 | 0 | NotAttempted |
| M110A_V05 | 55 | High | UI/map runtime needed | 4 | 130 | 0 | NotAttempted |
| M111A_V06 | 55 | High | UI/map runtime needed | 3 | 214 | 0 | NotAttempted |
| M112A_V08 | 55 | High | UI/map runtime needed | 7 | 423 | 0 | NotAttempted |
| M113A_V04 | 55 | High | UI/map runtime needed | 3 | 172 | 0 | NotAttempted |
| M114A_V02 | 55 | High | UI/map runtime needed | 3 | 139 | 0 | NotAttempted |
| M115A_V02 | 55 | High | UI/map runtime needed | 3 | 129 | 0 | NotAttempted |
| M116A_V03 | 55 | High | UI/map runtime needed | 4 | 168 | 0 | NotAttempted |
| M120A_V07 | 55 | High | UI/map runtime needed | 4 | 110 | 0 | NotAttempted |
| M121A_V02 | 55 | High | UI/map runtime needed | 3 | 56 | 0 | NotAttempted |
| M122A_V05 | 55 | High | UI/map runtime needed | 3 | 178 | 0 | NotAttempted |
| M123A_V02 | 55 | High | UI/map runtime needed | 3 | 61 | 0 | NotAttempted |
| M124A_V01 | 55 | High | UI/map runtime needed | 3 | 134 | 0 | NotAttempted |
| M125A_V02 | 55 | High | UI/map runtime needed | 3 | 74 | 0 | NotAttempted |
| M126A_V09 | 55 | High | UI/map runtime needed | 4 | 303 | 0 | NotAttempted |
| M127A_V01 | 55 | High | UI/map runtime needed | 4 | 92 | 0 | NotAttempted |
| M128A_V03 | 55 | High | UI/map runtime needed | 3 | 122 | 0 | NotAttempted |
| M129A_V06 | 55 | High | UI/map runtime needed | 3 | 151 | 0 | NotAttempted |
| M130A_V05 | 55 | High | UI/map runtime needed | 3 | 161 | 0 | NotAttempted |
| M131A_V01 | 55 | High | UI/map runtime needed | 4 | 99 | 0 | NotAttempted |
| M132A_V03 | 55 | High | UI/map runtime needed | 3 | 147 | 0 | NotAttempted |
| M133A_V02 | 55 | High | UI/map runtime needed | 3 | 207 | 0 | NotAttempted |
| M134A_V05 | 55 | High | UI/map runtime needed | 3 | 219 | 0 | NotAttempted |
| ME01A_V27 | 55 | High | UI/map runtime needed | 5 | 447 | 0 | NotAttempted |
| ME02A_V22 | 55 | High | UI/map runtime needed | 3 | 246 | 0 | NotAttempted |
| ME03A_V09 | 55 | High | UI/map runtime needed | 3 | 154 | 0 | NotAttempted |
| ME04A_V16 | 55 | High | UI/map runtime needed | 3 | 240 | 0 | NotAttempted |
| ME05A_V12 | 55 | High | UI/map runtime needed | 3 | 182 | 0 | NotAttempted |
| ME06A_V08 | 55 | High | UI/map runtime needed | 3 | 133 | 0 | NotAttempted |
| ME07A_V06 | 55 | High | UI/map runtime needed | 3 | 106 | 0 | NotAttempted |
| ME08A_V04 | 55 | High | UI/map runtime needed | 3 | 101 | 0 | NotAttempted |
| ME09A_V24 | 55 | High | UI/map runtime needed | 5 | 453 | 0 | NotAttempted |
| ME10A_V09 | 55 | High | UI/map runtime needed | 3 | 198 | 0 | NotAttempted |
| ME11A_V24 | 55 | High | UI/map runtime needed | 3 | 422 | 0 | NotAttempted |
| ME12A_V11 | 55 | High | UI/map runtime needed | 3 | 134 | 0 | NotAttempted |
| ME13A_V05 | 55 | High | UI/map runtime needed | 3 | 93 | 0 | NotAttempted |
| ME14A_V06 | 55 | High | UI/map runtime needed | 4 | 324 | 0 | NotAttempted |
| ME15A_V08 | 55 | High | UI/map runtime needed | 3 | 189 | 0 | NotAttempted |
| ME16A_V09 | 55 | High | UI/map runtime needed | 3 | 202 | 0 | NotAttempted |
| ME17A_V06 | 55 | High | UI/map runtime needed | 3 | 89 | 0 | NotAttempted |
| ME18A_V08 | 55 | High | UI/map runtime needed | 5 | 675 | 0 | NotAttempted |
| ME19A_V03 | 55 | High | UI/map runtime needed | 3 | 74 | 0 | NotAttempted |
| ME20A_V04 | 55 | High | UI/map runtime needed | 3 | 75 | 0 | NotAttempted |
| ME21A_V04 | 55 | High | UI/map runtime needed | 3 | 93 | 0 | NotAttempted |
| ME23A_V19 | 55 | High | UI/map runtime needed | 3 | 492 | 0 | NotAttempted |
| ME24A_V22 | 55 | High | UI/map runtime needed | 4 | 295 | 0 | NotAttempted |
| ME25A_V01 | 55 | High | UI/map runtime needed | 2 | 91 | 0 | NotAttempted |
| ME26A_V21 | 55 | High | UI/map runtime needed | 3 | 469 | 0 | NotAttempted |
| ME28A_V04 | 55 | High | UI/map runtime needed | 3 | 106 | 0 | NotAttempted |
| ME29A_V02 | 55 | High | UI/map runtime needed | 3 | 148 | 0 | NotAttempted |
| ME30A_V02 | 55 | High | UI/map runtime needed | 3 | 93 | 0 | NotAttempted |
| ME31A_V29 | 55 | High | UI/map runtime needed | 5 | 465 | 0 | NotAttempted |
| ME33A_V05 | 55 | High | UI/map runtime needed | 3 | 483 | 0 | NotAttempted |
| ME40A_V05 | 55 | High | UI/map runtime needed | 5 | 431 | 0 | NotAttempted |
| ME41A_V02 | 55 | High | UI/map runtime needed | 3 | 154 | 0 | NotAttempted |
| ME42A_V03 | 55 | High | UI/map runtime needed | 3 | 230 | 0 | NotAttempted |
| ME43A_V03 | 55 | High | UI/map runtime needed | 3 | 175 | 0 | NotAttempted |
| ME50A_V02 | 55 | High | UI/map runtime needed | 3 | 99 | 0 | NotAttempted |
| ME51A_V03 | 55 | High | UI/map runtime needed | 3 | 422 | 0 | NotAttempted |
| ME52A_V04 | 55 | High | UI/map runtime needed | 5 | 411 | 0 | NotAttempted |
| ME53A_V02 | 55 | High | UI/map runtime needed | 3 | 156 | 0 | NotAttempted |
| ME54A_V02 | 55 | High | UI/map runtime needed | 3 | 154 | 0 | NotAttempted |
| ME55A_V03 | 55 | High | UI/map runtime needed | 3 | 231 | 0 | NotAttempted |
| ME60A_V04 | 55 | High | UI/map runtime needed | 2 | 160 | 0 | NotAttempted |
| MI00A_V10 | 55 | High | UI/map runtime needed | 2 | 71 | 0 | NotAttempted |
| MI10A_V50 | 55 | High | UI/map runtime needed | 48 | 1058 | 0 | NotAttempted |
| MI30A_V12 | 55 | High | UI/map runtime needed | 4 | 101 | 0 | NotAttempted |
| MK00A_V08 | 55 | High | UI/map runtime needed | 2 | 61 | 0 | NotAttempted |
| MK10A_V13 | 55 | High | UI/map runtime needed | 14 | 521 | 0 | NotAttempted |
| MK20A_V12 | 55 | High | UI/map runtime needed | 28 | 477 | 0 | NotAttempted |
| MM00A_V07 | 55 | High | UI/map runtime needed | 10 | 454 | 0 | NotAttempted |
| NA40A_V57 | 55 | High | UI/map runtime needed | 60 | 2139 | 0 | NotAttempted |
| NA46A_V46 | 55 | High | UI/map runtime needed | 44 | 1942 | 0 | NotAttempted |
| PR13A_V03 | 55 | High | UI/map runtime needed | 26 | 978 | 0 | NotAttempted |
| RE01A_V14 | 55 | High | UI/map runtime needed | 2 | 235 | 0 | NotAttempted |
| RE31A_V03 | 55 | High | UI/map runtime needed | 2 | 133 | 0 | NotAttempted |
| RE81A_V12 | 55 | High | UI/map runtime needed | 2 | 176 | 0 | NotAttempted |
| RO01A04 | 55 | High | UI/map runtime needed | 3 | 222 | 0 | NotAttempted |
| RO02A_V05 | 55 | High | UI/map runtime needed | 5 | 299 | 0 | NotAttempted |
| RO03A_V06 | 55 | High | UI/map runtime needed | 6 | 346 | 0 | NotAttempted |
| S000A_V06 | 55 | High | UI/map runtime needed | 11 | 259 | 0 | NotAttempted |
| S002A_V05 | 55 | High | UI/map runtime needed | 9 | 176 | 0 | NotAttempted |
| S004A_V02 | 55 | High | UI/map runtime needed | 33 | 738 | 0 | NotAttempted |
| S006A_V05 | 55 | High | UI/map runtime needed | 11 | 267 | 0 | NotAttempted |
| S008A_V05 | 55 | High | UI/map runtime needed | 13 | 314 | 0 | NotAttempted |
| S020A_V10 | 55 | High | UI/map runtime needed | 9 | 957 | 0 | NotAttempted |
| S021A_V07 | 55 | High | UI/map runtime needed | 12 | 1128 | 0 | NotAttempted |
| S022A_V05 | 55 | High | UI/map runtime needed | 12 | 552 | 0 | NotAttempted |
| S023A_V05 | 55 | High | UI/map runtime needed | 9 | 486 | 0 | NotAttempted |
| S024A_V04 | 55 | High | UI/map runtime needed | 9 | 648 | 0 | NotAttempted |
| S025A_V08 | 55 | High | UI/map runtime needed | 18 | 1122 | 0 | NotAttempted |
| S026A_V02 | 55 | High | UI/map runtime needed | 6 | 252 | 0 | NotAttempted |
| S055A_V02 | 55 | High | UI/map runtime needed | 72 | 1528 | 0 | NotAttempted |
| S110A_V10 | 55 | High | UI/map runtime needed | 34 | 706 | 0 | NotAttempted |
| S111A_V04 | 55 | High | UI/map runtime needed | 75 | 1500 | 0 | NotAttempted |
| S113A_V05 | 55 | High | UI/map runtime needed | 108 | 2754 | 0 | NotAttempted |
| S151A_V10 | 55 | High | UI/map runtime needed | 13 | 381 | 0 | NotAttempted |
| S203 | 55 | High | UI/map runtime needed | 30 | 1128 | 0 | NotAttempted |
| S203A_V14 | 55 | High | UI/map runtime needed | 30 | 1128 | 0 | NotAttempted |
| S209A_V04 | 55 | High | UI/map runtime needed | 104 | 3088 | 0 | NotAttempted |
| S222A_V04 | 55 | High | UI/map runtime needed | 27 | 623 | 0 | NotAttempted |
| SA00A_V14 | 55 | High | UI/map runtime needed | 8 | 254 | 0 | NotAttempted |
| SE00A_V38 | 55 | High | UI/map runtime needed | 5 | 275 | 0 | NotAttempted |
| SE04A_V07 | 55 | High | UI/map runtime needed | 2 | 42 | 0 | NotAttempted |
| SE05A_V09 | 55 | High | UI/map runtime needed | 35 | 684 | 0 | NotAttempted |
| SE27A_V07X | 55 | High | UI/map runtime needed | 3 | 192 | 0 | NotAttempted |
| SL00A_V13 | 55 | High | UI/map runtime needed | 7 | 247 | 0 | NotAttempted |
| SPGB15_V06 | 55 | High | Extend EZE runtime | 39 | 590 | 0 | NotAttempted |
| SU15A_V05_NT | 55 | High | UI/map runtime needed | 12 | 414 | 0 | NotAttempted |
| SU15A_V09 | 55 | High | UI/map runtime needed | 12 | 444 | 0 | NotAttempted |
| ZZ90A_V10 | 55 | High | UI/map runtime needed | 3 | 298 | 0 | NotAttempted |
| D341A_V11 | 50 | Medium | Add characterization tests | 43 | 828 | 0 | NotAttempted |
| FUNKCIJE_STRINGI_S | 45 | Medium | Extend statement parser | 1 | 124 | 20 | NotAttempted |
| S560_V11 | 45 | Medium | Extend EZE runtime | 17 | 474 | 0 | NotAttempted |
| CE02A_V05 | 40 | Medium | Add characterization tests | 19 | 298 | 0 | NotAttempted |
| D239A_V20 | 40 | Medium | Add characterization tests | 53 | 717 | 0 | NotAttempted |
| D259A_V16 | 40 | Medium | Add characterization tests | 37 | 747 | 0 | NotAttempted |
| D260A_V13 | 40 | Medium | Add characterization tests | 36 | 718 | 0 | NotAttempted |
| D291A_V03 | 40 | Medium | Add characterization tests | 13 | 154 | 0 | NotAttempted |
| D293A_V16 | 40 | Medium | Add characterization tests | 44 | 1277 | 0 | NotAttempted |
| D320A_V04 | 40 | Medium | Add characterization tests | 59 | 1266 | 0 | NotAttempted |
| D323A_V04 | 40 | Medium | Add characterization tests | 21 | 691 | 0 | NotAttempted |
| D329A_V04 | 40 | Medium | Add characterization tests | 50 | 829 | 0 | NotAttempted |
| MI20A_V61 | 40 | Medium | UI/map runtime needed | 114 | 3239 | 0 | NotAttempted |
| PO55A | 40 | Medium | Add characterization tests | 33 | 2306 | 0 | NotAttempted |
| PR14A_V03 | 40 | Medium | Add characterization tests | 13 | 289 | 0 | NotAttempted |
| S523A_V09 | 40 | Medium | Add characterization tests | 5 | 81 | 0 | NotAttempted |
| S80 | 40 | Medium | Add characterization tests | 14 | 325 | 0 | NotAttempted |
| S805A_V03 | 40 | Medium | Add characterization tests | 14 | 325 | 0 | NotAttempted |
| SA46A_V03 | 40 | Medium | Add characterization tests | 10 | 162 | 0 | NotAttempted |
| SE06A_V05 | 40 | Medium | UI/map runtime needed | 64 | 1056 | 0 | NotAttempted |
| SE35A_V13 | 40 | Medium | Add characterization tests | 53 | 716 | 0 | NotAttempted |
| D270A_V10 | 35 | Medium | Extend EZE runtime | 8 | 962 | 0 | NotAttempted |
| D282A_V02 | 35 | Medium | Extend EZE runtime | 4 | 64 | 0 | NotAttempted |
| D285A_V01 | 35 | Medium | Extend EZE runtime | 5 | 90 | 0 | NotAttempted |
| D297A_V01 | 35 | Medium | Extend EZE runtime | 2 | 34 | 0 | NotAttempted |
| D305A_V03 | 35 | Medium | Extend EZE runtime | 1 | 33 | 0 | NotAttempted |
| S528M03 | 35 | Medium | Extend EZE runtime | 10 | 397 | 0 | NotAttempted |
| ZZ61A_V01 | 35 | Medium | Extend EZE runtime | 6 | 94 | 0 | NotAttempted |
| D266A_V05 | 30 | Medium | Add characterization tests | 9 | 103 | 0 | NotAttempted |
| D307A_V01 | 30 | Medium | Add characterization tests | 8 | 113 | 0 | NotAttempted |
| D322A_V07 | 30 | Medium | Add characterization tests | 21 | 291 | 0 | NotAttempted |
| SL17A_V23 | 30 | Medium | Add characterization tests | 32 | 748 | 0 | NotAttempted |
| ITEMS_28112006 | 25 | Medium | Add characterization tests | 0 | 0 | 0 | NotAttempted |
| S521A_V21 | 25 | Medium | Add characterization tests | 39 | 1729 | 0 | NotAttempted |
| SA41A_V20 | 25 | Medium | Add characterization tests | 33 | 1426 | 0 | NotAttempted |
| D315A_V01 | 15 | Low | Pilot candidate (verify with build) | 10 | 121 | 0 | NotAttempted |
| D336A_V03 | 15 | Low | Pilot candidate (verify with build) | 7 | 117 | 0 | NotAttempted |
| SPGBA_V02 | 15 | Low | Pilot candidate (verify with build) | 32 | 423 | 0 | NotAttempted |
| TE99W01 | 15 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| CALL_NAZIV_IZDELKA | 0 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| IMP01_V11 | 0 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| LINKAGE | 0 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| S617_V04 | 0 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| SE31M04 | 0 | Low | Pilot candidate (verify with build) | 7 | 84 | 0 | NotAttempted |
| TACEL | 0 | Low | Pilot candidate (verify with build) | 0 | 0 | 0 | NotAttempted |
| UU10A | 0 | Low | Pilot candidate (verify with build) | 8 | 10 | 0 | NotAttempted |

## 10. Build Failure Details

_Build phase not run._

