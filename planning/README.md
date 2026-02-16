# Planning Directory

This directory contains detailed business analysis and implementation planning documents for the Custom Monopoly project.

## Quick Start

**New to the project?** Start here:
1. Read [MASTER-ROADMAP.md](./MASTER-ROADMAP.md) for executive overview
2. Review [IMPLEMENTATION-PLAN.md](./IMPLEMENTATION-PLAN.md) for Phase 1 details
3. Check [IMPLEMENTATION-PLAN-PHASES-2-6.md](./IMPLEMENTATION-PLAN-PHASES-2-6.md) for remaining phases

**Ready to develop?** Follow the phase-by-phase implementation plan starting with Phase 1.

## Purpose

The `/planning` directory is used for:
- Feature planning documents
- Epic breakdowns
- User stories and acceptance criteria
- Implementation roadmaps
- Business analysis documentation
- Detailed requirements specifications

## Relationship with Specifications

The `/planning` and `/specifications` directories serve different purposes:

### `/specifications` (Architecture & Design)
- **Purpose**: High-level system architecture and design decisions
- **Created by**: Software Architect agents
- **Focus**: Technical architecture, patterns, technology choices
- **Audience**: Architects, technical leads, senior developers
- **Examples**: System architecture, ADRs, design patterns

### `/planning` (Requirements & Implementation Plans)
- **Purpose**: Detailed feature requirements and implementation plans
- **Created by**: Business Analyst agents
- **Focus**: What to build, user stories, acceptance criteria, implementation details
- **Audience**: Developers, QA engineers, product managers
- **Examples**: Feature plans, user stories, test scenarios

## Directory Structure

```
planning/
├── README.md                                   # This file
├── MASTER-ROADMAP.md                           # ✅ Executive overview and timeline
├── IMPLEMENTATION-PLAN.md                      # ✅ Phase 1 detailed plan
├── IMPLEMENTATION-PLAN-PHASES-2-6.md          # ✅ Phases 2-6 detailed plans
├── epics/                                      # Epic-level planning documents (future)
│   ├── epic-001-core-gameplay.md
│   └── epic-002-mod-system.md
├── features/                                   # Feature-level planning documents (future)
│   ├── feature-001-game-board.md
│   ├── feature-002-player-management.md
│   └── feature-003-ai-opponents.md
└── roadmap/                                    # Additional roadmap details (future)
    └── project-roadmap.md
```

### Current Planning Documents (Completed)

#### Master Documents
1. **[MASTER-ROADMAP.md](./MASTER-ROADMAP.md)** - Executive summary, timeline, all phases overview
2. **[IMPLEMENTATION-PLAN.md](./IMPLEMENTATION-PLAN.md)** - Phase 1 (Foundation & Core Architecture) in detail
3. **[IMPLEMENTATION-PLAN-PHASES-2-6.md](./IMPLEMENTATION-PLAN-PHASES-2-6.md)** - Phases 2-6 in detail

These documents provide a complete, iterative implementation plan for the entire 18-week development cycle.

## Document Types

### 1. Epic Planning Documents
High-level groupings of related features that deliver significant business value.

**Location**: `planning/epics/`
**Naming**: `epic-XXX-short-title.md`

**Contains**:
- Epic overview and business objectives
- List of features within the epic
- User personas and value proposition
- Success metrics
- Implementation timeline
- Dependencies and risks

### 2. Feature Planning Documents
Detailed specifications for individual features ready for implementation.

**Location**: `planning/features/`
**Naming**: `feature-XXX-short-title.md`

**Contains**:
- Feature overview and business value
- User stories with acceptance criteria
- Functional and non-functional requirements
- Unity implementation guidance
- Test scenarios
- Examples from similar implementations
- Dependencies and prerequisites

### 3. Roadmap Documents
Project timelines, sprint planning, and release schedules.

**Location**: `planning/roadmap/`

**Contains**:
- Project milestones
- Sprint breakdowns
- Release schedules
- Resource allocation
- Risk tracking

## Who Creates Documents

Planning documents are created by the **Senior Business Analyst** agent:
- Specializes in Unity game projects
- Reviews specifications before planning
- Asks for clarification when needed
- Creates detailed, executable documentation
- Does NOT write code

To use the Senior Business Analyst agent:
```
@senior-business-analyst Please create a feature plan for [feature name]
```

## Writing Guidelines

### For All Planning Documents

1. **Be Specific**: Use concrete metrics and examples, not vague statements
2. **Include Examples**: Reference successful Unity games with similar features
3. **Make It Executable**: Developers should be able to start coding from your document
4. **Document Uncertainties**: List open questions and assumptions clearly
5. **Link to Specifications**: Reference architecture docs in `/specifications`

### User Story Format

```markdown
### User Story: [Title]
**As a** [user type]
**I want to** [action]
**So that** [benefit]

**Acceptance Criteria:**
- [ ] Specific, testable criterion
- [ ] Specific, testable criterion
- [ ] Specific, testable criterion

**Priority**: [High | Medium | Low]
**Estimated Complexity**: [Simple | Medium | Complex]
```

### Requirement Format

```markdown
#### Requirement: [Name]
**Description**: [Detailed description]
**Rationale**: [Why this is needed]
**Dependencies**: [What must exist first]

**Behavior Specification:**
- Given [precondition]
- When [action]
- Then [expected result]

**Edge Cases:**
- [Edge case and handling]
```

## Quality Standards

All documents in this directory should meet these standards:

### Completeness
- All sections filled with meaningful content
- No placeholder text or TODOs
- Open questions explicitly documented
- Assumptions clearly stated

### Specificity
- Concrete metrics instead of "fast" or "good"
- Specific Unity components mentioned
- Clear acceptance criteria
- Testable requirements

### Executability
- Developers can start coding immediately
- All necessary details included
- Integration points identified
- File organization specified

### Examples
- Reference games with similar features
- Sample user flows
- UI/UX examples
- Data structure examples (conceptual)

## Workflow

### Creating a New Feature Plan

1. **Review Specifications**
   - Read related architecture documents in `/specifications`
   - Understand existing system design
   - Identify integration points

2. **Research**
   - How do successful Unity games implement this?
   - What Unity packages or assets exist?
   - What are common pitfalls?

3. **Clarify Requirements**
   - Ask questions about ambiguities
   - Confirm scope and priorities
   - Validate assumptions with stakeholders

4. **Create Document**
   - Use the feature planning template
   - Fill in all sections thoroughly
   - Include examples and references
   - Document open questions

5. **Review**
   - Can a developer build this without further questions?
   - Are all acceptance criteria testable?
   - Are Unity-specific details included?
   - Are risks and dependencies documented?

### Using Planning Documents

**For Developers:**
1. Read the feature plan before starting implementation
2. Check acceptance criteria to understand "done"
3. Follow Unity implementation guidance
4. Refer to linked specifications for architecture context
5. Flag any ambiguities or questions

**For QA Engineers:**
1. Use acceptance criteria for test planning
2. Create test cases from test scenarios
3. Verify edge cases are covered
4. Validate performance requirements

**For Project Managers:**
1. Track progress against acceptance criteria
2. Monitor dependencies and risks
3. Update roadmaps based on actuals
4. Coordinate stakeholder reviews

## Integration with Development

Planning documents should be:
- **Living documents**: Updated as requirements evolve
- **Referenced in code**: Link back to planning docs in code comments where relevant
- **Validated during QA**: Test against acceptance criteria
- **Reviewed post-implementation**: Update with lessons learned

## Document Status

Use these status values:

| Status | Meaning |
|--------|---------|
| **Draft** | Work in progress, not ready for review |
| **Review** | Ready for stakeholder review |
| **Approved** | Approved and ready for implementation |
| **In Progress** | Currently being implemented |
| **Complete** | Implementation finished and verified |
| **Archived** | Superseded or no longer relevant |

## Quality Checklist

Before marking a planning document as "Approved":

- [ ] Document is in the correct `/planning` subdirectory
- [ ] All sections are complete with meaningful content
- [ ] User stories have clear acceptance criteria
- [ ] Requirements are specific and testable
- [ ] Non-functional requirements include metrics
- [ ] Unity implementation guidance is provided
- [ ] Examples from successful games are included
- [ ] Test scenarios are comprehensive
- [ ] Open questions are documented
- [ ] Assumptions are explicit
- [ ] Dependencies are identified
- [ ] Related specifications are linked
- [ ] A developer can start coding from this document

## Common Pitfalls to Avoid

❌ **Vague Requirements**
- Don't: "The feature should perform well"
- Do: "The feature should maintain 60 FPS with up to 100 active objects"

❌ **Missing Examples**
- Don't: Just describe requirements abstractly
- Do: Include examples from games like "Unity Game X does this by..."

❌ **Unclear Acceptance Criteria**
- Don't: "Feature works correctly"
- Do: "When player clicks button, modal opens within 0.2 seconds showing accurate data"

❌ **No Unity Context**
- Don't: Generic software requirements
- Do: Specify Unity components, prefabs, ScriptableObjects, scene structure

❌ **Assumptions Not Documented**
- Don't: Make silent assumptions
- Do: Explicitly list all assumptions in the document

## Getting Help

### Questions About Planning
Contact the Senior Business Analyst agent for:
- Creating new planning documents
- Clarifying requirements
- Breaking down epics into features
- Researching similar implementations

### Questions About Architecture
Refer to `/specifications` directory and Software Architect for:
- System architecture decisions
- Design patterns
- Technology stack choices
- Cross-cutting concerns

### Questions About Implementation
Developers should:
1. Review both planning and specifications
2. Flag any ambiguities or conflicts
3. Suggest updates to planning docs based on implementation learnings

## Tools and Resources

### Unity-Specific Resources
- [Unity Manual](https://docs.unity3d.com/Manual/index.html)
- [Unity Best Practices](https://unity.com/how-to)
- [Unity Asset Store](https://assetstore.unity.com/)

### Requirements and Planning
- [User Story Template](https://www.mountaingoatsoftware.com/agile/user-stories)
- [Acceptance Criteria Best Practices](https://www.altexsoft.com/blog/acceptance-criteria/)
- [Feature Planning Guide](https://www.productplan.com/glossary/feature/)

### Unity Game Examples
- Unity Learn tutorials
- GDC talks on Unity development
- Unity blog post-mortems

## Notes

⚠️ **This directory should NOT contain code** - It's for planning and requirements documentation only

✅ **This directory SHOULD contain**:
- Feature plans with user stories
- Detailed requirements specifications
- Implementation roadmaps
- Test scenarios and acceptance criteria
- Examples and references
- Business analysis documentation

## Revision History

| Date | Version | Changes | Author |
|------|---------|---------|--------|
| 2026-02-16 | 1.0 | Initial planning directory creation | Senior Business Analyst Agent |

---

**Last Updated**: 2026-02-16
**Maintained By**: Senior Business Analyst Team
